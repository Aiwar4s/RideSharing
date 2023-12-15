const PROXY_CONFIG = [
  {
    "/api": {
      "target":"https://dolphin-app-j6hoh.ondigitalocean.app",
      "secure": false,
      "pathRewrite": {
        "^/api": ""
      },
      "logLevel": "debug"
    },
    "changeOrigin": true,
  }
]

module.exports = PROXY_CONFIG;
