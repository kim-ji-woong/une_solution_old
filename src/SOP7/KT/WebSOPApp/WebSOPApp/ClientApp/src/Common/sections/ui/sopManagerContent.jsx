import React, { Component } from 'react';
import { Link } from 'react-router-dom';

import styles from '../../Common/css/style.module.css';
import SopManagerResource from '../resource/id';
import SopManager from './sopManager';

class SopManagerContent extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);

		this.props = props;
	}

	onClickMenu(e) {
		var target = e;

		if (target.innerText === SopManager.menu.editSOP) {
			this.props.content(SopManager.menu.editSOP, null);
		}
		else if (target.innerText === SopManager.menu.open) {
			this.props.content(SopManager.menu.open, null);
		}
		else if (target.innerText === SopManager.menu.save) {
			this.props.content(SopManager.menu.save, this.props.sopData, true);
		}
		else if (target.innerText === SopManager.menu.saveXML) {
			this.props.content(SopManager.menu.saveXML, this.props.sopData);
		}
		else if (target.innerText === SopManager.menu.openXML) {
			this.props.content(SopManager.menu.openXML, null);
		}
		else if (target.innerText === SopManager.menu.newSOP) {
			this.props.content(SopManager.menu.newSOP, null);
		}
	}

	render() {
		return (
			<div className={SopManagerContent.cssStyles.saLeft}>
				<div className={SopManagerContent.cssStyles.aslWrap + " " + SopManagerContent.cssStyles.typeC}>
					{
						/*<Link to="/" className={SopManagerContent.cssStyles.salHome}>{SopManagerResource.ID.home}</Link>*/
					}
					<div className={SopManagerContent.cssStyles.salMenu + " " + SopManagerContent.cssStyles.on}>
						<a onClick={(e) => this.onClickMenu(e.target)} className={SopManagerContent.cssStyles.salIco + " " + SopManagerContent.cssStyles.ico0201}>{SopManagerResource.ID.menu.editSOP}</a>
						<dl className={SopManagerContent.cssStyles.salCont}>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.newSOP}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.open}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.save}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.saveAs}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.delete}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.openXML}</a></dd>
							<dd><a className={SopManagerContent.cssStyles.clickable} onClick={(e) => this.onClickMenu(e.target)}>{SopManager.menu.saveXML}</a></dd>
						</dl>
					</div>
				</div>
			</div>
		);
	}
}

export default SopManagerContent;