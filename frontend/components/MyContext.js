import React, { Component } from 'react';
/*

export const values = {
  'loggedIn':false,
};

console.log("!!!!!!!!!!!!!!!!!! Exporting !!!!!!!!!!!!!!!!!! ")

export default  React.createContext(
  'normal'
);*/


export const themes = {
  light: {
    foreground: '#ffffff',
    background: '#222222',
  },
  dark: {
    foreground: '#000000',
    background: '#eeeeee',
  },
};

export const ThemeContext = React.createContext(
  themes.light // default value
);


/*
// first we will make a new context
const MyContext = React.createContext();

// Then create a provider Component
class MyProvider extends Component {
  state = {
    loggedIn: false,
  }
  render() {
    return (
      <MyContext.Provider value={{
        loggedIn: this.state.loggedIn,
        setLoggedIn: (isLoggedIn) => this.setState({
            loggedIn: isLoggedIn
        })
      }}>
        {this.props.children}
      </MyContext.Provider>
    )
  }
}

export default function() {
  return {
    Provider:   MyProvider,
    Context:  MyContext,
  }
}*/