# MT5 trading server watchdog #

Watchdog utility, which will monitor trading activity in real-time and will log any suspicious behavior
- for now connection to MT5 server(s) needs to be configured (see Connection section in app.config)
- by default reconnects to trading server on error 5 times before giving up (see Connection section in app.config)
- suspicious activity is logged to ./logs/ (see Logging section in app.config)
- suspicious activity is partially configurable (see Validation section in app.config)
- by default deals are retained for 3 seconds (see Monitoring section in app.config)

Potential improvements
- better logging with NLog or similar
- use durable/persistent queues like RabbitMQ
- more and better configuration options