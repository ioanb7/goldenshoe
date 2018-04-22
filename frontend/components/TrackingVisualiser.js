import fetch from 'isomorphic-unfetch'

import React from 'react'// using an ES6 transpiler, like babel
import { render } from 'react-dom'
import config from '../config.js'

class TrackingVisualiser extends React.Component {
  constructor(props) {
    super(props);
    this.state = {'id' : props.id, 'loading': true, 'progress': 0}
  }

  componentDidMount() {
    var self = this;
    fetch(`${config.baseUrl}/api/tracking/${this.state.id}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage["jwt"] }
    })
        .then(res => res.json())
        .then(data => {
            console.log(data)
            
            self.setState({
                'loading':false,
                'progress':data.progress,
                'progress_p': "" + data.progress + "%",
                'currentLocation': data.currentLocation,
                'estimatedArrival': data.estimatedArrival
            });
        })
        .catch(function(error) {
            console.log(error);
        });
  }
    
  
  render() {
      if(this.state.loading == true) {
          return (<div><p>Tracking data is loading..</p></div>);
      }

    return (
        <div>
            <h3>TrackingVisualiser</h3>
            <p>Your order is now in: <span>{this.state.currentLocation}</span></p>
            <p>Estimated arrival: <span>{this.state.estimatedArrival}</span></p>
            <p className="progress_text">Progress is: <span>{this.state.progress_p}</span></p>
            <progress className="progress is-large is-success" value={this.state.progress} max="100">{this.state.progress_p}</progress>
            
        <style jsx>{`
        .progress_text {
            margin-top:20px;
        }
    `}</style>
        </div>
    );
  }
}

export default TrackingVisualiser