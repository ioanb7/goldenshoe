import Layout from '../components/MyLayout.js'
import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import { ThemeContext } from '../components/MyContext.js'
import config from '../config.js'
import Router from 'next/router'

class Logout extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    delete localStorage["jwt"];
    window.setLoggedIn(false) // <HACK>
    const href = `/`
    Router.push(href, href, { shallow: true })
  }
    
  
  render() { //onClick={this.accountClick}
    return (
    
    <Layout>
      <p>Logging you out..</p>
    </Layout>
    );
  }
}

export default Logout