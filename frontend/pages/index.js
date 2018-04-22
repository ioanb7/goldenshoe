import Layout from '../components/MyLayout.js'
import Link from 'next/link'
import fetch from 'isomorphic-unfetch'
import config from '../config.js'


const Shoe = (props) => (
    
    <div className="product">
        <Link as={`/p/${props.product.id}`} href={`/post?id=${props.product.id}`}>
        <div>
            <a>
                <h1>{props.product.title}</h1>
                <div>
                    {props.product.description}
                </div>
                    
                    { props.product.images.length ? (
                        <div className="parent">
                            <img src={props.product.images[0].filename}/>
                        </div>
                    ) : <p>Missing Image :(</p>}
            </a>
            
            <p>$<span>{props.product.price}</span></p>
            <a className="button is-primary">BUY NOW</a>
            
        </div>
        </Link>

        
    <style jsx>{`
    
    h1 { 
        font-size:2em;
        text-align:center;
        margin: 1.5em 0em;
    }
    
        .product {
            display: inline-flex;
            width: 48%;
            padding: 20px;
            background-color: #f5f5f5;
            margin: 5px;
        }
        .button {
            float:right;
        }
        `}
        </style>
    </div>
);

const Index = (props) => (
  <Layout>
  
  
  
    <h1>Shoes</h1>
    <ul>
      {props.rows.map(row => (
        <div className="row" key={row.id}>
            <Shoe product={row.shoe[0]}/>
            <Shoe product={row.shoe[1]}/>
        </div>
      ))}
    </ul>
    
    <style jsx>{`
    
    h1 { 
        font-size:2em;
        text-align:center;
        margin: 1.5em 0em;
    }
    
        a, p {
            display:block;
            margin-bottom:10px;
            text-align:center;
        }
        a.button {
            display: block;
            margin: 0 auto;
        }
        img {
            margin:0 auto!important;
            display:block!important;
            max-width: 100%;
            max-height: 100%;
        }
        .parent {

        }

        .row {
            background-color:#f5f5f5;
        }

        
    `}</style>
    
    
  </Layout>
)

Index.getInitialProps = async function() {
  const res = await fetch(`${config.baseUrl}/api/products`)
  const data = await res.json()
  //console.log(data)
  //console.log(`Show data fetched. Count: ${data.length}`)
  for(var dd in data) {
      if(data[dd].images.length > 0) {
        data[dd].images[0].filename = "/uploads/products/" + data[dd].id + "/1.jpg"
      }
      //
      //data[dd].
  }

  console.log(config)
  var c = 1;
  var rows = []
  var row = []
  var i = 0
  for(var dd in data) {
    if(c == 1) {
        row.push(data[dd])
    } else {
        row.push(data[dd])
        rows.push({
            shoe: row,
            id: i
        })
        row = []
    }
    c = c + 1
    if(c == 3) c = 1;
    i = i + 1
  }
  if(row.length != 0) {
      rows.push(row) // odd number
  }

  return {
    rows: rows
  }
}

export default Index