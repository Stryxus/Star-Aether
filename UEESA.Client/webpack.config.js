const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const nodeExternals = require('webpack-node-externals');
const ImageMinimizerPlugin = require('image-minimizer-webpack-plugin');

var config = {
    devtool: 'source-map',
    entry: [
        path.resolve(__dirname, 'wwwroot-dev', 'js', 'bundle.js'),
        path.resolve(__dirname, 'wwwroot-dev', 'css', 'bundle.scss')
    ],
    externals: [nodeExternals()],
    output: {
        filename: 'bundle.min.js',
        path: path.resolve(__dirname, 'wwwroot'),
    },
    plugins: [
        new CleanWebpackPlugin(),
        new ImageMinimizerPlugin({
            minify: ImageMinimizerPlugin.squooshMinify,
            filter: (source, sourcePath) => {
                console.log("Processing Image: " + sourcePath);
                return true;
            },
            filename: '[path][name].webp',
            minimizerOptions: {
                encodeOptions: {
                    webp: {
                        quality: 50,
                        target_size: 0,
                        target_PSNR: 0,
                        method: 6,
                        sns_strength: 50,
                        filter_strength: 60,
                        filter_sharpness: 0,
                        filter_type: 1,
                        partitions: 0,
                        segments: 4,
                        pass: 1,
                        show_compressed: 0,
                        preprocessing: 0,
                        autofilter: 1,
                        partition_limit: 0,
                        alpha_compression: 1,
                        alpha_filtering: 1,
                        alpha_quality: 100,
                        lossless: 0,
                        exact: 0,
                        image_hint: 0,
                        emulate_jpeg_size: 0,
                        thread_level: 1,
                        low_memory: 1,
                        near_lossless: 100,
                        use_delta_palette: 0,
                        use_sharp_yuv: 0,
                    },
                    /*
                    avif: {
                        cqLevel: 33,
                        cqAlphaLevel: -1,
                        denoiseLevel: 0,
                        speed: 6,
                        subsample: 1,
                        chromaDeltaQ: false,
                        sharpness: 0,
                    }
                    */
                },
            }
        })
    ],
    optimization: {
        minimize: true,
        minimizer: [
            new TerserPlugin({
                parallel: true,
                extractComments: false,
                terserOptions: {
                    mangle: false,
                    module: false,
                    ie8: false,
                    safari10: false,
                    format: {
                        comments: false,
                    },
                },
            }),
        ],
    },
};

module.exports = (env, argv) => {
    if (argv.mode === 'production') {

        config.module = {
            rules: [
                {
                    test: /\.scss$/,
                    use: [
                        {
                            loader: 'file-loader',
                            options: { outputPath: '', name: 'bundle.[hash].min.css' }
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                sourceMap: true
                            }
                        }
                    ],
                }
            ]
        }

    } else if (argv.mode === 'development') {

        config.module = {
            rules: [
                {
                    test: /\.png$/,
                    type: 'asset'
                },
                {
                    test: /\.scss$/,
                    use: [
                        {
                            loader: 'file-loader',
                            options: { outputPath: '', name: 'bundle.min.css' }
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                sourceMap: true
                            }
                        }
                    ],
                }
            ]
        }
    }

    return config;
};