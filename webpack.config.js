const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const extractCSS = new ExtractTextPlugin('site.min.css');

module.exports = {
    entry: {
        'main': './wwwroot/main.js'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: 'site.min.js',
        publicPath: 'dist/'
    },
    plugins: [
        extractCSS,
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
            'window.$': 'jquery',
            Popper: ['popper.js', 'default']
        }),
        new webpack.optimize.UglifyJsPlugin()
    ],
    module: {
        rules: [{
                test: /\.css$/,
                use: extractCSS.extract(['css-loader?minimize'])
            },
            {
                test: /\.js?$/,
                use: 'babel-loader'
            },
            //{ test: /\.js?$/, use: { loader: 'babel-loader', options: { presets: ['@babel/preset-react', '@babel/preset-env'] } } },
        ]
    }
};