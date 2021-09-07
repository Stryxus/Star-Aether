import { resolve, dirname, sep } from 'path'
import { constants, readFileSync, writeFileSync, statSync, accessSync, truncateSync, unlinkSync, readdirSync, renameSync, mkdirSync, copyFileSync } from 'fs'
import { execSync } from 'child_process'

import chokidar from 'chokidar'
import getFolderSize from 'get-folder-size'
import sass from 'sass'
import { minify } from 'terser'
import ffmpeg from 'ffmpeg-static'
import commandExistsSync from 'command-exists'
//import { ImagePool } from '@squoosh/lib'
import imagemin from 'imagemin'
import imageminAvif from 'imagemin-avif'

// Global Variables //

const __dirname = resolve()
const __client_dirname = __dirname + sep + 'UEESA.Client'
const __client_wwwroot_dirname = __client_dirname + sep + 'wwwroot'
const __client_wwwrootdev_dirname = __client_dirname + sep + 'wwwroot-dev'
const __config_filename = __dirname + sep + 'ueesa-config.json'
const __cache_filename = __dirname + sep + 'ueesa-cache.json'

var isDebug

//const imagePool = new ImagePool()

// Global Variables //

// JSON //

var config =
{
    jsDependencies: []
}

var cacheEntities =
{
    cached: []
}

// JSON //

function fileExists(filepath)
{
    try
    {
        accessSync(filepath, constants.R_OK | constants.W_OK)
        return true
    }
    catch (e)
    {
        return false
    }
}

function findFiles(path)
{
    const entries = readdirSync(path, { withFileTypes: true })
    const files = entries.filter(file => !file.isDirectory()).map(file => ({ ...file, path: path + sep + file.name }));
    const folders = entries.filter(folder => folder.isDirectory())
    for (const folder of folders)
    {
        files.push(...findFiles(`${path}${sep}${folder.name}${sep}`))
    }
    return files
}

function filterFiles(files, ext)
{
    return Object.values(files).filter(file => String(file.name).split('.').pop() == ext)
}

function loadConfig()
{
    if (fileExists(__config_filename))
    {
        config = JSON.parse(readFileSync(__config_filename))
    }
    else
    {
        writeFileSync(__config_filename, JSON.stringify(config, null, "\t"), 'utf8')
    }
}

function addCacheEntry(filepath)
{
    const relativeFilePath = filepath.replace(__client_wwwrootdev_dirname, '')
    cacheEntities.cached = cacheEntities.cached.filter(f => f.relativeFilePath !== relativeFilePath)
    cacheEntities.cached.push({ relativeFilePath: relativeFilePath, lastModified: statSync(filepath).mtime })
    if (fileExists(__cache_filename))
    {
        truncateSync(__cache_filename, 0)
    }
    writeFileSync(__cache_filename, JSON.stringify(cacheEntities, null, "\t"), 'utf8')
}

function needsCaching(filepath, procExt)
{
    if (fileExists(__cache_filename))
    {
        cacheEntities = JSON.parse(readFileSync(__cache_filename))
    }
    const procFilePath = filepath.replace('wwwroot-dev', 'wwwroot')
    const containsEntry = cacheEntities.cached.some(file => file.relativeFilePath === filepath.replace(__client_wwwrootdev_dirname, ''))
    if (!containsEntry)
    {
        addCacheEntry(filepath)
    }
    return !(containsEntry && fileExists(procFilePath.substring(0, procFilePath.lastIndexOf('.')) + '.' + procExt))
}

function needsProcessing(filepath)
{
    const cachedFile = cacheEntities.cached.filter(file => file.relativeFilePath === filepath.replace(__client_wwwrootdev_dirname, ''))[0]
    const needsProc = new Date(cachedFile.lastModified).getTime() < statSync(filepath).mtime.getTime()
    if (needsProc)
    {
        addCacheEntry(filepath)
    }
    return needsProc
}

async function processJS(jsFiles)
{
    const minJSFilePath = __client_wwwroot_dirname + sep + 'bundle.min.js'
    const minMapFilePath = __client_wwwroot_dirname + sep + 'bundle.js.map'
    console.log('  | Minifying JS: ' + minJSFilePath.replace(__client_wwwroot_dirname, ''))
    var orderedJS = 'var GLOBAL = {};' // Define this to silence errors made from Blazor's GLOBAL variable missing
    config.jsDependencies.forEach(async item =>
    {
        const fullPath = __dirname + sep + item
        if (item.substring(item.lastIndexOf('.')) == '.js' && fileExists(fullPath))
        {
            console.log('  | - Including Dependency: ' + item)
            orderedJS += readFileSync(item, 'utf8') + '\n'
        }
        else
        {
            console.error('  | - Dependency "' + item + '" does not exist or is not a .js file - skipping...')
        }
    })
    jsFiles.forEach(async item =>
    {
        if (item.name == 'service-worker.js' || item.name == 'service-worker.published.js')
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Minifying Service Worker: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            const result = await minify(readFileSync(item.path, 'utf8') , { sourceMap: false, module: false, mangle: false, ecma: 2021, compress: true })
            writeFileSync(minJSFilePath, result.code, 'utf8')
        }
        else
        {
            orderedJS += readFileSync(item.path, 'utf8') + '\n'
        }
    })
    const result = await minify(orderedJS, { sourceMap: true, module: false, mangle: false, ecma: 2021, compress: !isDebug })
    if (fileExists(minJSFilePath))
    {
        truncateSync(minJSFilePath, 0)
    }
    if (fileExists(minMapFilePath))
    {
        truncateSync(minMapFilePath, 0)
    }
    writeFileSync(minJSFilePath, result.code, 'utf8')
    writeFileSync(minMapFilePath, result.map, 'utf8')
}

function processSASS(scssFile)
{
    const minCSSFilePath = __client_wwwroot_dirname + sep + 'bundle.min.css'
    const minMapFilePath = __client_wwwroot_dirname + sep + 'bundle.css.map'
    console.log('  | Minifying SASS: ' + minCSSFilePath.replace(__client_wwwroot_dirname, ''))
    const result = sass.renderSync({
        file: scssFile, sourceMap: true, outFile: 'bundle.css', outputStyle: isDebug ? 'expanded' : 'compressed', indentType: 'tab', indentWidth: 1, quietDeps: true, includePaths: [__client_wwwrootdev_dirname + sep + 'css' + sep + 'thirdparty']
    })
    if (fileExists(minCSSFilePath))
    {
        truncateSync(minCSSFilePath, 0)
    }
    if (fileExists(minMapFilePath))
    {
        truncateSync(minMapFilePath, 0)
    }
    writeFileSync(minCSSFilePath, result.css.toString(), 'utf8')
    writeFileSync(minMapFilePath, result.map.toString(), 'utf8')
}

async function processing()
{
    console.log('\\  Changes Made > Running Bundle Pass...')
    console.log(' \\')

    const files = findFiles(__client_wwwrootdev_dirname)
    const jsFiles = filterFiles(files, 'js').filter(file => String(dirname(file.path)).split(sep).pop() == 'js')
    const scssFile = __client_wwwrootdev_dirname + sep + 'css' + sep + 'bundle.scss'
    const htmlFiles = filterFiles(files, 'html')
    const svgFiles = filterFiles(files, 'svg')
    const jsonFiles = filterFiles(files, 'json')
    const woff2Files = filterFiles(files, 'woff2')
    const ttfFiles = filterFiles(files, 'ttf')
    const pngFiles = filterFiles(files, 'png')
    const mp4Files = filterFiles(files, 'mp4')

    await processJS(jsFiles)
    processSASS(scssFile)

    htmlFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'html') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying HTML: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            copyFileSync(item.path, output)
        }
    })

    svgFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'svg') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying SVG: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            copyFileSync(item.path, output)
        }
    })

    jsonFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'json') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying JSON: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            copyFileSync(item.path, output)
        }
    })

    woff2Files.forEach(item =>
    {
        if (needsCaching(item.path, 'woff2') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying Font: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            copyFileSync(item.path, output)
        }
    })

    ttfFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'ttf') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying Font: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            copyFileSync(item.path, output)
        }
    })

    pngFiles.forEach(async item =>
    {
        if (needsCaching(item.path, 'avif') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot').replace('.png', '.avif')
            console.log('  | Transcoding Image: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            if (fileExists(item.path.replace('wwwroot-dev', 'wwwroot')))
            {
                unlinkSync(item.path.replace('wwwroot-dev', 'wwwroot'))
            }
            await imagemin([item.path],
            {
                destination: dirname(output),
                plugins:
                [
                    imageminAvif({ quality: 66, speed: isDebug ? 5 : 0 })
                ]
            })
            renameSync(output.replace('.avif', '.png'), output)
            /*
            try
            {
                const image = imagePool.ingestImage(item.path)
                await image.decoded
                await image.encode({
                    avif:
                    {
                        cqLevel: 45,
                        cqAlphaLevel: -1,
                        denoiseLevel: 0,
                        tileColsLog2: 0,
                        tileRowsLog2: 0,
                        speed: 0,
                        subsample: 1,
                        chromaDeltaQ: false,
                        sharpness: 0,
                        tune: 0
                    }
                })
                const result = (await image.encodedWith.avif).binary
                writeFileSync(output, result)
            }
            catch (e)
            {
                console.error(e)
            }
            */
        }
    })

    mp4Files.forEach(item =>
    {
        if (needsCaching(item.path, 'webm') || needsProcessing(item.path))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot').replace('.mp4', '.webm')
            console.log('  | Transcoding Video: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            if (commandExistsSync('ffmpeg'))
            {
                execSync('start cmd /C ffmpeg -y -i ' + item.path + (isDebug ? ' -c:v librav1e -rav1e-params speed=10:low_latency=true' : ' -c:v librav1e -b:v 200K -rav1e-params speed=0:low_latency=true') + ' -movflags +faststart -c:a libopus -q:a 128 ' + output, err =>
                {
                    if (err)
                    {
                        throw err
                    }
                })
            }
            else
            {
                console.error('No non-GPL compliant FFmpeg build detected in enviroment variables - falling back to libaom, video transcoding will take substantially longer and will be much lower quality!')
                execSync('start cmd /C ' + ffmpeg + ' -y -i ' + item.path + ' -c:v libaom-av1 ' + (isDebug ? '-crf 52' : '-crf 30 -b:v 200k') + ' -movflags +faststart -c:a libopus -q:a 128 ' + output, err =>
                {
                    if (err)
                    {
                        throw err
                    }
                })
            }
        }
    })

    const inputSize = await getFolderSize.loose(__client_wwwrootdev_dirname)
    const outputSize = await getFolderSize.loose(__client_wwwroot_dirname)
    console.log('  |')
    console.log('  | > Size Before: ' + inputSize.toLocaleString('en') + ' bytes')
    console.log('  | > Size After:  ' + outputSize.toLocaleString('en') + ' bytes')
    console.log('  | > Compression: ' + (100 - (outputSize / inputSize * 100)).toFixed(4).toString() + '%')
    console.log(' /')
    console.log('/')
    console.log('\n----------------------------------------------------------------------------------------------------\n')
}

(async () =>
{
    // Dramatic Intro
    console.clear()
    console.log('####################################################################################################')
    console.log('##                                                                                                ##')
    console.log('##                                       UEESA Core Bundler                                       ##')
    console.log('##                                                                                                ##')
    console.log('##                                  By Connor \'Stryxus\' Shearer.                                  ##')
    console.log('##                                                                                                ##')
    console.log('####################################################################################################\n')
    //

    process.argv.forEach(item =>
    {
        switch (item)
        {
            case 'build:debug':
                isDebug = true
                break
            case 'build:release':
                isDebug = false
                break
        }
    })

    loadConfig()
    await processing()
    chokidar.watch(__client_wwwrootdev_dirname, { awaitWriteFinish: true }).on('change', async () =>
    {
        await processing()
    });
})()