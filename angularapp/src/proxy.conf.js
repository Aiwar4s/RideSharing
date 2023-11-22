const PROXY_CONFIG = [
  {
    context: [
      "/api/drivers",
    ],
    target: "https://dolphin-app-j6hoh.ondigitalocean.app/",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
