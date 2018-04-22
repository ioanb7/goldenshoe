import Layout from '../components/MyLayout.js'
import Link from 'next/link'
import fetch from 'isomorphic-unfetch'

const Index = (props) => (
  <Layout>
  
  
  
    <h1>Shoes</h1>
    <ul>
      {props.shows.map(show => (
        <li key={show.id}>
          <Link as={`/p/${show.id}`} href={`/post?id=${show.id}`}>
            <div>
                <a>
                    <h1>{show.title}</h1>
                    <div>
                        {show.description}
                    </div>
                        
                        { show.images.length ? (
                    <img src={show.images[0].filename}/>
                        ) : <p>Missing Image :(</p>}
                </a>
                
                <p>$<span>{show.price}</span></p>
                <a className="button is-primary">BUY NOW</a>
                
            </div>
          </Link>
        </li>
      ))}
    </ul>
    
    <style jsx>{`
    
    h1 { 
        font-size:2em;
        text-align:center;
        margin: 1.5em 0em;
    }
    
        li {
            display: inline-block;
            width: 48%;
            padding: 20px;
            background-color: #f5f5f5;
            margin: 5px;
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
        }
    `}</style>
    
    
  </Layout>
)

Index.getInitialProps = async function() {
  const res = await fetch('http://192.168.99.100:80/api/products')
  const data = await res.json()
  console.log(data)
  console.log(`Show data fetched. Count: ${data.length}`)
  return {
    shows: data
  }
}

export default Index