import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'
import Link from 'next/link'

var onerror = (e) => {
    e.target.style.display='none'
}

const Post =  (props) => (
    <Layout>
    
        <h1>{props.show.title}</h1>
        <div>
            <h3>Description</h3>
            <div>
                {props.show.description}
            </div>
        </div>
        <div className="gallery">
            { props.show.images.length ? (
                <img src={props.show.images[0].filename} onError={onerror}/>
            ) : (
                <p>Missing Image :(</p>
            )}
        </div>
    
        <p>$<span>{props.show.price}</span></p>
        <p>Stock total:<span>{props.show.stock}</span></p>
        <p>Reserved: <span>{props.show.reserved}</span></p>
        <p>&nbsp;=>&nbsp;Stock available right now: <span>{props.show.stock - props.show.reserved}</span></p>
        <p>Shoe size: <span>{props.show.shoeSize}</span>&nbsp;
            <Link href="/print" target="_blank">
                <a className="">Is this correct for me?</a>
            </Link>
        </p>

        {(props.show.stock - props.show.reserved > 0) ? (
            <Link href={props.show.buyUrl}>
                <a className="button is-primary">BUY NOW</a>
            </Link>
        ) : (
            <Link href="#">
                <a className="button is-primary" disabled>BUY NOW</a>
            </Link>
        )}

        
        <style jsx>{`
      
          .gallery {
              
          }
          .gallery img {
            max-width:500px;
            max-height:500px;
            display:inline-block;
            margin:0 auto;
          }
      `}</style>
    </Layout>
)

Post.getInitialProps = async function (context) {
  const { id } = context.query
  const res = await fetch(`http://192.168.99.100:80/api/products/${id}`)
  const show = await res.json()
  console.log(`Fetched show: ${show.title}`)

  show.buyUrl = "/create-order/" + show.id
  return { show }
}

export default Post