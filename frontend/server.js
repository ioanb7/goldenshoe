const express = require('express')
const next = require('next')

const dev = process.env.NODE_ENV !== 'production'
const app = next({ dev })
const handle = app.getRequestHandler()

app.prepare()
.then(() => {
  const server = express()

  server.get('/p/:id', (req, res) => {
    const actualPage = '/post'
    const queryParams = { id: req.params.id }
    app.render(req, res, actualPage, queryParams)
  })
  server.get('/create-order/:id', (req, res) => {
    const actualPage = '/create-order'
    const queryParams = { id: req.params.id }
    app.render(req, res, actualPage, queryParams)
  })
  server.get('/process-order/:id', (req, res) => {
    const actualPage = '/process-order'
    const queryParams = { id: req.params.id }
    app.render(req, res, actualPage, queryParams)
  })
  server.get('/print-download', (req, res) => {
    var file = __dirname + '/uploads/PDF_men_windows_chrome.pdf';
    res.download(file); // Set disposition and send it.
  })
  server.get('/order/:id', (req, res) => {
    const actualPage = '/order'
    const queryParams = { id: req.params.id }
    app.render(req, res, actualPage, queryParams)
  })

  server.get('*', (req, res) => {
    return handle(req, res)
  })

  server.listen(3000, (err) => {
    if (err) throw err
    console.log('> Ready on http://localhost:3000')
  })
})
.catch((ex) => {
  console.error(ex.stack)
  process.exit(1)
})