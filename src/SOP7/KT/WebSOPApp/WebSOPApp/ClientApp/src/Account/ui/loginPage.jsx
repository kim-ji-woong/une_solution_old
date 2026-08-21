import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

class LoginPage extends Component {

	constructor(props) {
		super(props);
	}

	render() {
		this.props.history.push("/sop-simulator");

		return (
			<>
			</>
		);
	}
}

export default withRouter(LoginPage);