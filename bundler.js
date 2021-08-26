import { resolve, dirname, sep } from 'path'
import { constants, readFileSync, writeFileSync, statSync, accessSync, truncateSync, unlinkSync, readdirSync, renameSync, mkdirSync, copyFileSync } from 'fs'
import { execSync } from 'child_process'

import chokidar from 'chokidar'
import getFolderSize from 'get-folder-size'
import imagemin from 'imagemin'
import imageminAvif from 'imagemin-avif'
import sass from 'sass'
import { minify } from 'terser'
import ffmpeg from 'ffmpeg-static'

// Global Variables //

const __dirname = resolve()
const __client_dirname = __dirname + sep + 'UEESA.Client'
const __client_wwwroot_dirname = __client_dirname + sep + 'wwwroot'
const __client_wwwrootdev_dirname = __client_dirname + sep + 'wwwroot-dev'
const __cache_filename = __dirname + sep + 'ueesa-cache.json'

// Global Variables //

// JSON //

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

function needsCaching(filepath, procExt)
{
    if (fileExists(__cache_filename))
    {
        cacheEntities = JSON.parse(readFileSync(__cache_filename))
    }
    const procFilePath = filepath.replace('wwwroot-dev', 'wwwroot')
    const containsEntry = cacheEntities.cached.some(file => file.relativeFilePath == filepath.replace(__client_wwwrootdev_dirname, ''))
    if (!containsEntry)
    {
        cacheEntities.cached.push({ relativeFilePath: filepath.replace(__client_wwwrootdev_dirname, ''), lastModified: statSync(filepath).mtime })
        if (fileExists(__cache_filename))
        {
            truncateSync(__cache_filename, 0)
        }
        writeFileSync(__cache_filename, JSON.stringify(cacheEntities, null, "\t"), 'utf8')
    }
    return !(containsEntry && fileExists(procFilePath.substring(0, procFilePath.lastIndexOf('.')) + '.' + procExt))
}

async function processJS(jsFiles)
{
    const minJSFilePath = __client_wwwroot_dirname + sep + 'bundle.min.js'
    const minMapFilePath = __client_wwwroot_dirname + sep + 'bundle.js.map'
    console.log('  | Minifying JS: ' + minJSFilePath.replace(__client_wwwroot_dirname, ''))
    var orderedJS = 'var GLOBAL = {}\n' // Define this to silence errors made from Blazor's GLOBAL variable missing
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
    const result = await minify(orderedJS, { sourceMap: true, module: false, mangle: false, ecma: 2021, compress: true })
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
        file: scssFile, sourceMap: true, outFile: 'bundle.css.map', outputStyle: "compressed", indentType: "tab", indentWidth: 1, quietDeps: true, includePaths: [__client_wwwrootdev_dirname + sep + 'css' + sep + 'thirdparty']
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
    const pngFiles = filterFiles(files, 'png')
    const mp4Files = filterFiles(files, 'mp4')
    const scssFile = __client_wwwrootdev_dirname + sep + 'css' + sep + 'bundle.scss'
    const jsFiles = filterFiles(files, 'js')
    const htmlFiles = filterFiles(files, 'html')
    const svgFiles = filterFiles(files, 'svg')
    const jsonFiles = filterFiles(files, 'json')

    await processJS(jsFiles)
    processSASS(scssFile)

    htmlFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'html'))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying HTML: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            copyFileSync(item.path, output)
        }
    })

    svgFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'svg'))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying SVG: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            copyFileSync(item.path, output)
        }
    })

    jsonFiles.forEach(item =>
    {
        if (needsCaching(item.path, 'json'))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot')
            console.log('  | Copying JSON: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            copyFileSync(item.path, output)
        }
    })

    pngFiles.forEach(async item =>
    {
        if (needsCaching(item.path, 'avif'))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot').replace('.png', '.avif')
            console.log('  | Transcoding Image: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            if (fileExists(item.path.replace('wwwroot-dev', 'wwwroot')))
            {
                unlinkSync(item.path.replace('wwwroot-dev', 'wwwroot'))
            }
            await imagemin([item.path],
            {
                destination: dirname(output),
                plugins:
                [
                    imageminAvif({ quality: 40, speed: 0 })
                ]
                })
            renameSync(output.replace('.avif', '.png'), output)
        }
    })

    mp4Files.forEach(item =>
    {
        if (needsCaching(item.path, 'webm'))
        {
            const output = item.path.replace('wwwroot-dev', 'wwwroot').replace('.mp4', '.webm')
            console.log('  | Transcoding Video: ' + item.path.replace(__client_wwwrootdev_dirname, '') + ' > ' + output.replace(__client_wwwroot_dirname, ''))
            mkdirSync(dirname(output), { recursive: true })
            // Change CRF to 5 once librav1e is able to be used
            execSync('start cmd /C ' + ffmpeg + ' -y -i ' + item.path + ' -c:v libaom-av1 -crf 50 -b:v 500k -movflags +faststart -c:a libopus -q:a 128 ' + output, err =>
            {
                if (err)
                {
                    throw err
                }
            })
        }
    })

    const inputSize = await getFolderSize.loose(__client_wwwrootdev_dirname)
    const outputSize = await getFolderSize.loose(__client_wwwroot_dirname)
    console.log('  |')
    console.log('  | > Size Before: ' + inputSize.toLocaleString('en') + ' bytes')
    console.log('  | > Size After: ' + outputSize.toLocaleString('en') + ' bytes')
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

    await processing()
    chokidar.watch(__client_wwwrootdev_dirname, { awaitWriteFinish: true }).on('change', async () =>
    {
        await processing()
    });
})()