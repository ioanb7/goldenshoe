import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import TrackingVisualiser from '../components/TrackingVisualiser.js'
import config from '../config.js'

class Order extends React.Component {
  constructor(props) {
    super(props);
    this.state = {'id' : props.id}
  }

  componentDidMount() {
    var self = this;
    fetch(`${config.baseUrl}/api/order/${this.state.id}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
    .then(res => res.json())
    .then(json => {
        console.log(json)
    })
    .catch(function(error) {
        console.log(error);
    });
  }
    
  
  render() {
    return (
      <Layout>
        <h1>Order</h1>
        <TrackingVisualiser id={this.state.id}/>
      </Layout>
    );
  }
}
Order.getInitialProps = async function (context) {
  const { id } = context.query
  return {id}
}

export default Order