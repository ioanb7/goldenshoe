import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import Link from 'next/link'
import config from '../config.js'

class Orders extends React.Component {
  constructor(props) {
    super(props);
    this.state = {'orders': [], 'loading':true}
  }

  componentDidMount() {
    var self = this;
    fetch(`${config.baseUrl}/api/order`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
    .then(res => res.json())
    .then(json => {
        console.log(json)


        self.setState({
          'loading': false,
          'orders': json.orders
        })
    })
    .catch(function(error) {
        console.log(error);
    });
  }
    
  
  render() {


    if(this.state.loading == true) {
      return (<Layout><div><p>The data about your orders is loading..</p></div></Layout>);
    }
    if(this.state.orders.length < 1) {
      return (<Layout><div><p>No orders so far.</p></div></Layout>);
    }

    var statuses = [
      "Created",
      "Processing",
      "On the way..", //"ProcessedSuccess",
      "ProcessedFail",
      "Delivered"
    ]

    return (
      <Layout>
        <h1>Your Orders</h1>
      
        <div>
        {this.state.orders.map(function(d, idx){

          var link_ = "/order/" + d.id;
          return (
            <li key={d.id}>
              <Link href={link_}>
                <a>
                  Order <span>{d.id}</span>&nbsp;
                  <span>{statuses[d.status-1]}</span>
                </a>
              </Link>
              
              <style jsx>{`
                div {
                    display:inline;
                }
              `}</style>
            </li>
          )
        })}
      </div>

      </Layout>
    );
  }
}

export default Orders