const path = require('path')
const webpack = require('webpack')
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const OptimizeCSSPlugin = require('optimize-css-assets-webpack-plugin')

module.exports = () => {
  console.log('Building vendor files for \x1b[33m%s\x1b[0m', process.env.NODE_ENV)

  const isDevBuild = !(process.env.NODE_ENV && process.env.NODE_ENV === 'production')
  const extractCSS = new ExtractTextPlugin('vendor.css')

  return [{
    stats: { modules: false },
    resolve: {
      extensions: ['.js']
    },
    module: {
      rules: [
        { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' },
        { test: /\.css(\?|$)/, use: extractCSS.extract(['css-loader']) }
      ]
    },
    entry: {
      vendor: ['bootstrap', 'bootstrap/dist/css/bootstrap.css', 'jquery']
    },
    output: {
      path: path.join(__dirname, 'wwwroot', 'dist'),
      publicPath: '/dist/',
      filename: '[name].js',
      library: '[name]_[hash]'
    },
    plugins: [
      extractCSS,
      // Compress extracted CSS.
      new OptimizeCSSPlugin({
        cssProcessorOptions: {
          safe: true
        }
      }),
      new webpack.ProvidePlugin({
        $: 'jquery',
        jQuery: 'jquery',
        Popper: ['popper.js', 'default']
        /* For modal, you will need to add tether */
      }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
    ].concat(isDevBuild ? [] : [
      new webpack.optimize.UglifyJsPlugin()
    ])
  }]
}
