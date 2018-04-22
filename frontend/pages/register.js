import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import config from '../config.js'

class Login extends React.Component {
  constructor(props) {
    super(props);
    
    this.state = {name:'',username: '',password: '',password2: '',email: ''};
    this.handleUsernameChange = this.handleUsernameChange.bind(this);
    this.handlePasswordChange = this.handlePasswordChange.bind(this);
    this.handlePassword2Change = this.handlePassword2Change.bind(this);
    this.handleEmailChange = this.handleEmailChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {
  }
    
  
  handleUsernameChange(event) {
    this.setState({username: event.target.value});
  }
  handleNameChange(event) {
    this.setState({name: event.target.value});
  }
  handlePasswordChange(event) {
    this.setState({password: event.target.value});
  }
  handlePassword2Change(event) {
    this.setState({password2: event.target.value});
  }
  handleEmailChange(event) {
    this.setState({email: event.target.value});
  }
  
  handleSubmit(event) {
    event.preventDefault();
    var data = {
      'Name': this.state.name,
      'Username': this.state.username,
      'Password': this.state.password,
      'Password2': this.state.password2,
      'Email': this.state.email
    }
      
    fetch(`${config.baseUrl}/api/authenticator/Register`, {
        method: 'POST',
        body:    JSON.stringify(data),
        headers: { 'Content-Type': 'application/json' }
    })
        .then(res => res.json())
        .then(json => console.log(json))
        .catch(function(error) {
            console.log(error);
        });
      
  }
  
  
  render() { //onClick={this.accountClick}
    return (
    <Layout>
    
      <form onSubmit={this.handleSubmit}>
        <h1>Register</h1>
        
        <div className="field">
          <p className="control has-icons-left has-icons-right">
            <input className="input" type="name" placeholder="Alex Jones (name)" value={this.state.name} onChange={this.handleNameChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-id-card"></i>
            </span>
          </p>
        </div>

        <div className="field">
          <p className="control has-icons-left has-icons-right">
            <input className="input" type="username" placeholder="Username" value={this.state.username} onChange={this.handleUsernameChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-id-card"></i>
            </span>
          </p>
        </div>
        
        <div className="field">
          <p className="control has-icons-left has-icons-right">
            <input className="input" type="email" placeholder="Email" value={this.state.email} onChange={this.handleEmailChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-envelope"></i>
            </span>
          </p>
        </div>
        
        <div className="field">
          <p className="control has-icons-left">
            <input className="input" type="password" placeholder="Password" value={this.state.password} onChange={this.handlePasswordChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-lock"></i>
            </span>
          </p>
        </div>
        
        <div className="field">
          <p className="control has-icons-left">
            <input className="input" type="password" placeholder="Password (repeat)" value={this.state.password2} onChange={this.handlePassword2Change}/>
            <span className="icon is-small is-left">
              <i className="fas fa-lock"></i>
            </span>
          </p>
        </div>
        
        <div className="field">
          <p className="control">
            <button className="button is-success">
              Login
            </button>
          </p>
        </div>
        
      </form>
    </Layout>
    );
  }
}

export default Login