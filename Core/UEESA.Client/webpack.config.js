const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const CopyPlugin = require("copy-webpack-plugin");
const nodeExternals = require('webpack-node-externals');
const ImageMinimizerPlugin = require('image-minimizer-webpack-plugin');

var config = {
    devtool: 'source-map',
    entry: './wwwroot-dev/js/bundle.js',
    externals: [nodeExternals()],
    output: {
        filename: 'bundle.[contenthash].min.js',
        path: path.resolve(__dirname, 'wwwroot'),
    },
    plugins: [
        new CleanWebpackPlugin(),
        new HtmlWebpackPlugin({
            filename: path.resolve('wwwroot', 'index.html'),
            inject: true,
            template: path.resolve('wwwroot-dev', 'index.html'),
        }),
        new MiniCssExtractPlugin({
            filename: 'bundle.[contenthash].min.css'
        }),
        new ImageMinimizerPlugin({
            minify: ImageMinimizerPlugin.squooshMinify,
            test: /\.(png)$/i,
            filename: "[name].webp",
            minimizerOptions: {
                encodeOptions: {
                    webp: {
                        quality: 75,
                        target_size: 0,
                        target_PSNR: 0,
                        method: 4,
                        sns_strength: 50,
                        filter_strength: 60,
                        filter_sharpness: 0,
                        filter_type: 1,
                        partitions: 0,
                        segments: 4,
                        pass: 1,
                        show_compressed: 0,
                        preprocessing: 0,
                        autofilter: 0,
                        partition_limit: 0,
                        alpha_compression: 1,
                        alpha_filtering: 1,
                        alpha_quality: 100,
                        lossless: 0,
                        exact: 0,
                        image_hint: 0,
                        emulate_jpeg_size: 0,
                        thread_level: 0,
                        low_memory: 0,
                        near_lossless: 100,
                        use_delta_palette: 0,
                        use_sharp_yuv: 0,
                    }
                    /*
                    avif: {
                        cqLevel: 33,
                        cqAlphaLevel: -1,
                        denoiseLevel: 0,
                        speed: 6,
                        subsample: 1,
                        chromaDeltaQ: false,
                        sharpness: 0,
                    },
                    */
                },
            },
        }),
    ],
    optimization: {
        minimize: true,
        minimizer: [
            new OptimizeCssAssetsPlugin({
                cssProcessorOptions: {
                    map: {
                        inline: false,
                        annotation: true,
                    },
                },
            }),
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
                            loader: 'style-loader',
                        },
                        {
                            loader: 'css-loader',
                            options: {
                                sourceMap: true,
                                importLoaders: 2
                            }
                        },
                        {
                            loader: 'resolve-url-loader',
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
                    test: /\.scss$/,
                    use: [
                        {
                            loader: 'style-loader',
                        },
                        {
                            loader: 'css-loader',
                            options: {
                                sourceMap: true,
                                importLoaders: 2
                            }
                        },
                        {
                            loader: 'resolve-url-loader',
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