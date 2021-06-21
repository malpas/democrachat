const { createProxyMiddleware } = require("http-proxy-middleware");

module.exports = function (app) {
    app.use(
        createProxyMiddleware("/api", {
            target: "http://localhost:5000/",
            proxyTimeout: 200000
        })
    );

    app.use(
        createProxyMiddleware("/hub", {
            target: "http://localhost:5000/",
            changeOrigin: true,
            proxyTimeout: 200000
        })
    )

    app.use(
        createProxyMiddleware("/hub/chat", {
            target: "ws://localhost:5000/",
            ws: true,
            changeOrigin: true,
            proxyTimeout: 200000,
        })
    )

    app.use(
        createProxyMiddleware("/static", {
            target: "http://localhost:1235/"
        })
    )
};