import { Header, NavBar} from './Header'
import Head from 'next/head'
import {ThemeContext, themes} from './MyContext.js'


const layoutStyle = {
  //margin: 20,
  //padding: 20,
  //border: '1px solid #DDD'
}

function Toolbar(props) {
  return (
    <NavBar onClick={props.changeTheme}>
      Change Theme
    </NavBar>
  );
}

class Layout extends React.Component {
  constructor(props) {
    super(props);


    this.props = props;
    var self = this;

    this.state = {
      theme: themes.light,
      loggedIn: false,
    };
    
    if (process.env.OS == undefined) { // browser. <HACK>
      window.setLoggedIn = (value) => {
        self.setState(state => ({
          loggedIn:
            value
        }));
      }
    }

    this.toggleTheme = () => {
      this.setState(state => ({
        theme:
          state.theme === themes.dark
            ? themes.light
            : themes.dark,
      }));
    };
  }

  componentDidMount () {
    //TODO: make request to the server to check if the JWT is still valid. if it is not, switch to not logged in template and redirect back to /home

    if (process.env.OS == undefined) { // browser. <HACK>
    
      this.setState(state => ({
        loggedIn:
          localStorage["jwt"] != null
      }));
    }
  }

  
  render() {
    return (
    <ThemeContext.Provider value={this.state}>
      <Toolbar changeTheme={this.toggleTheme} />
    <div style={layoutStyle}>
      <Head>
        <title>My page title</title>
        <meta name="viewport" content="width=device-width, initial-scale=1"/>
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.2/css/bulma.min.css"/>
          <script defer src="https://use.fontawesome.com/releases/v5.0.7/js/all.js"></script>
      </Head>
    
      <Header />
      
      <div className="columns">
        <div className="column is-four-fifths">
        <div className="main_container">
              {this.props.children}
        </div>
      </div>
        <div className="column sidebar">
          <h3>About us</h3>
          <p>Fine quality shoes</p>
        </div>
      </div>
      
      
      <footer className="footer">
    <div className="container">
      <div className="content has-text-centered">
        <p>
          <strong><i>a</i></strong> Golden Shoe Shop experiment.
        </p>
      </div>
    </div>
  </footer>

  <style jsx>{`
      
          div.sidebar { 
              margin-top:50px;
          }
          .main_container {
            padding:20px;
            margin:20px;
          }
          .footer {
            padding: 3rem 1.5rem 3rem;
          }
      `}</style>


      <style jsx global>{`
      
      h1 { 
        font-size:3em;
        margin: 0em 0em 0.6em 0px;
    }
    h2 { 
        font-size:2.25em;
    }
    h3 { 
        font-size:1.75em;
    }

    form {
      max-width: 500px;
      margin:0 auto;
    }
    .content p {
    margin-top:1em;
  }

      `}</style>
    </div>
    
    </ThemeContext.Provider>
  );
  }
}

export default Layout