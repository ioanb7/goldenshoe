import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import Router from 'next/router'
import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'

class ProcessOrder extends React.Component {
  constructor(props) {
    super(props);
    this.state = {'id' : parseInt(props.id), cardId: '', cardCode: 450, loading:true}

    
    this.handleCardIdChange = this.handleCardIdChange.bind(this);
    this.handleCardCodeChange = this.handleCardCodeChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  
  handleCardIdChange(event) {
    this.setState({cardId: event.target.value});
  }
  handleCardCodeChange(event) {
    this.setState({cardCode: parseInt(event.target.value)});
  }

  componentDidMount() {
    //get order
    var self = this;
    fetch(`http://192.168.99.100/api/order/${this.state.id}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
    .then(res => res.json())
    .then(json => {
        console.log(json)
        self.setState({
          loading: false,
          price: json.price
        })
    })
    .catch(function(error) {
        console.log(error);
    });
  }

  
  handleSubmit(event) {
    event.preventDefault();

    var data = {
      'OrderId': this.state.id,
      'CardId': this.state.cardId,
      'CardCode': this.state.cardCode,
    }
    debugger;
      
    fetch('http://192.168.99.100/api/order/process', {
        method: 'POST',
        body:    JSON.stringify(data),
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
        .then(res => res.json())
        .then(json => {
            console.log(json)

            debugger;
            const href = `/order/${json.orderId}`
            Router.push(href, href, { shallow: true })
        })
        .catch(function(error) {
            console.log(error);
        });
  }
    
  
  render() { //onClick={this.accountClick}


  if(this.state.loading == true) {
    return (<Layout><div><p>The data about your orders is loading..</p></div></Layout>);
  }

    return (
    <Layout>
      <form onSubmit={this.handleSubmit}>
        <h1>Order <span>{this.state.id}</span></h1>
        <p>Price: $<span>{this.state.price}</span></p>
        <div className="field">
          <p className="control has-icons-left has-icons-right">
            <input className="input" type="username" placeholder="XXXX XXXX XXXX XXXX"  value={this.state.cardId} onChange={this.handleCardIdChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-credit-card"></i>
            </span>
          </p>
        </div>
        <div className="field">
          <p className="control has-icons-left">
          <input className="input" type="number" placeholder="XXX" min="100" max="999" value={this.state.cardCode} onChange={this.handleCardCodeChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-lock"></i>
            </span>
          </p>
        </div>
        <div className="field">
          <p className="control">
            <button className="button is-success">
              Process Order
            </button>
          </p>
        </div>
        </form>

    
    </Layout>
    );
  }
}
ProcessOrder.getInitialProps = async function (context) {
  const { id } = context.query
  return {id}
}

export default ProcessOrder