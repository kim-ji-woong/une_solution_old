import React, { Component } from 'react';
import '../css/app.css';

class SMComponentMenuBtn extends Component {
    onClickMenu = () =>
    {
        this.props.onClickMenu(this.props.menu.dataType);
    }

    render() {
        const { isActive, dataType, menuName } = this.props.menu;
        const className = isActive ? "menuBtn active" : "menuBtn";

        return (
            <li className={className} data-type={dataType} onClick={this.onClickMenu}>
                {menuName}
            </li>
        );
    }
}

export default SMComponentMenuBtn;