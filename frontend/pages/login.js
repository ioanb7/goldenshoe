import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import { ThemeContext } from '../components/MyContext.js'
import config from '../config.js'

class Login extends React.Component {
  constructor(props) {
    super(props);
    this.state = {username: '',password: ''};
    this.handleUsernameChange = this.handleUsernameChange.bind(this);
    this.handlePasswordChange = this.handlePasswordChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleUsernameChange(event) {
    this.setState({username: event.target.value});
  }
  handlePasswordChange(event) {
    this.setState({password: event.target.value});
  }
  componentDidMount() {

  }
    
  handleSubmit(event) {
    event.preventDefault();
    var data = {
      'Username': this.state.username,
      'Password': this.state.password,
    }

      
    fetch(`${config.baseUrl}/api/authenticator/login`, {
        method: 'POST',
        body:    JSON.stringify(data),
        headers: { 'Content-Type': 'application/json' }
    })
        .then(res => res.json())
        .then(json => {
            localStorage["jwt"] = json.apiKey;
            window.setLoggedIn(true) // <HACK>
        })
        .catch(function(error) {
            console.log(error);
        });
      
  }
  
  
  render() { //onClick={this.accountClick}
    return (
    
    <Layout>
      <form onSubmit={this.handleSubmit}>
        <h1>Login</h1>
        <div className="field">
          <p className="control has-icons-left has-icons-right">
            <input className="input" type="username" placeholder="Username"  value={this.state.username} onChange={this.handleUsernameChange}/>
            <span className="icon is-small is-left">
              <i className="fas fa-id-card"></i>
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