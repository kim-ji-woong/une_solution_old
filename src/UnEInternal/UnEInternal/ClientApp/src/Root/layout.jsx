import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './navMenu';
import { VacationMenus } from '../Vacation/vacationMenus';
import { VacationBody } from '../Vacation/vacationBody';
import $ from 'jquery';
//import './css/NavMenu.css';
//import styles from '../Vacation/css/vacation.module.css';



export class Layout extends Component {

    constructor(props) {
        super(props);

        this.props = props;

        this.state = {
            disUI: this.displayUI(),
        }

    }

    resizeUI() {
        this.setState({ disUI: this.displayUI() });
    }

    componentDidMount() {
        window.addEventListener('resize', () => this.resizeUI());
    }


    static displayName = Layout.name;


    displayUI = () => {
        let displayUI = [];

        let widthSize = window.outerWidth;


        if (widthSize < 768) { //모바일
            displayUI.push(
                <>
                    <div>
                        <div>
                            {this.props.children}
                        </div>
                    </div>
                </>
            );

        } else if (640 <= widthSize && widthSize <= 959) { //가로 모바일
            displayUI.push(
                <>
                    <div>
                        <div>
                            {this.props.children}
                        </div>
                    </div>
                </>
            );

        } else if (768 <= widthSize && widthSize <= 1024) { //태블릿
            displayUI.push(
                <>
                    <div>
                        <NavMenu loginUser={this.props.loginUser} onLogin={this.props.onLogin} options={this.props.options} onLogout={this.props.onLogout} />
                        <Container>
                            {this.props.children}
                        </Container>
                    </div>
                </>
            );

        } else if (960 <= widthSize && widthSize <= 1280) { //가로 태블릿
            displayUI.push(
                <>
                    <div>
                        <NavMenu loginUser={this.props.loginUser} onLogin={this.props.onLogin} options={this.props.options} onLogout={this.props.onLogout} />
                        <Container>
                            {this.props.children}
                        </Container>
                    </div>
                </>
            );

        } else if (widthSize >= 1025) { //데스크탑
            displayUI.push(
                <>
                    <div>
                        <NavMenu loginUser={this.props.loginUser} onLogin={this.props.onLogin} options={this.props.options} onLogout={this.props.onLogout} />
                        <Container>
                            {this.props.children}
                        </Container>
                    </div>
                </>
            );
        } else {
            displayUI.push(
                <></>
            );
        }

        return displayUI;
    }

    render() {
        setTimeout(() => { this.resizeUI() }, 500);
        let displayUI = this.state.disUI;

        return (
          <>
            {displayUI}
          </>
        );
  }
}
