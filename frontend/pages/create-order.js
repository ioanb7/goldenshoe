import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import Router from 'next/router'
import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'

class CreateOrder extends React.Component {
  constructor(props) {
    super(props);
    this.state = {'id' : parseInt(props.id)}
  }

  componentDidMount() {
    var data = {
      'ProductId': this.state.id,
    }
      
    fetch('http://192.168.99.100/api/order', {
        method: 'POST',
        body:    JSON.stringify(data),
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
        .then(res => res.json())
        .then(json => {
            console.log(json)

            const href = `/process-order/${json.orderId}`
            Router.push(href, href, { shallow: true })
        })
        .catch(function(error) {
            console.log(error);
        });
  }
    
  
  render() { //onClick={this.accountClick}
    return (
    <Layout>
        <p>Loading..</p>
    </Layout>
    );
  }
}
CreateOrder.getInitialProps = async function (context) {
  const { id } = context.query
  return {id}
}

export default CreateOrder