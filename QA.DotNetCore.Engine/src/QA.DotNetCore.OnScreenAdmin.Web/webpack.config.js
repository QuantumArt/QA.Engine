/* eslint-disable import/no-extraneous-dependencies, global-require, no-undef, quote-props */
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const webpack = require('webpack');
const path = require('path');
const WebpackNotifierPlugin = require('webpack-notifier');
// const HtmlWebpackPlugin = require('html-webpack-plugin');
// const CleanWebpackPlugin = require('clean-webpack-plugin');

// const HtmlWebpackPluginConfig = new HtmlWebpackPlugin({
//   // template: './ClientApp/index.html',
//   // filename: 'index.html',
//   // inject: 'body',
//   title: 'Caching',
// });

console.log(process.env.NODE_ENV);

module.exports = {
  entry: {
    main: [
      'babel-polyfill',
      './ClientApp/index.js',
    ],
  },
  output: {
    path: path.resolve(__dirname, 'wwwroot/dist'),
    filename: '[name].js',
    publicPath: '/dist/',
    // devtoolModuleFilenameTemplate: info => path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
  },
  resolve: {
    extensions: ['.js', '.jsx'],
    modules: [
      'node_modules',
      path.resolve(__dirname, 'ClientApp'),
    ],
  },
  // devtool: 'cheap-module-source-map',
  module: {
    rules: [{
      enforce: 'pre',
      test: /\.(js|jsx)?$/,
      exclude: /node_modules/,
      loader: 'eslint-loader',
      options: {
        failOnError: false,
      },
    },
    {
      test: /\.(js|jsx)?$/,
      use: ['react-hot-loader/webpack', 'babel-loader'],
      exclude: /node_modules/,
    },
    {
      test: /\.css$/,
      use: ['style-loader', 'css-loader'],
    },
    {
      test: /\.(woff|woff2|eot|ttf|otf)?$/,
      use: 'url-loader',
    },
    {
      test: /\.(png|gif)?$/,
      use: 'url-loader',
    },
    {
      test: /\.svg$/,
      use: [{
        loader: 'url-loader',
      },
      {
        loader: 'svgo-loader',
        options: {
          plugins: [{
            removeTitle: true,
          },
          {
            convertColors: {
              shorthex: false,
            },
          },
          {
            convertPathData: false,
          },
          ],
        },
      },
      ],
    },
    {
      test: /\.(jpg|jpeg)?$/,
      use: 'file-loader?name=[name].[ext]&outputPath=images/',
    },
    ],
  },
  plugins: [
    new WebpackNotifierPlugin(),
    new webpack.NamedModulesPlugin(),
    // new CleanWebpackPlugin(['dist/build']),

    // HtmlWebpackPluginConfig,
    // new webpack.HashedModuleIdsPlugin(),
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: JSON.stringify(process.env.NODE_ENV),
      },
    }),
    new BundleAnalyzerPlugin({
      analyzerMode: 'static',
      openAnalyzer: false,
    }),

    new webpack.optimize.CommonsChunkPlugin({
      name: 'vendor',
      minChunks(module, count) {
        const context = module.context;
        return context && context.indexOf('node_modules') >= 0;
      },
    }),
    new webpack.SourceMapDevToolPlugin({
      filename: '[name].js.map',
      moduleFilenameTemplate: info => path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
      exclude: ['vendor.js'],
    }),


  ],
};
