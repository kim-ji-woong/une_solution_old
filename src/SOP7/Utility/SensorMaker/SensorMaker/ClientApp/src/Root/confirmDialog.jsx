import React, { Component } from 'react';
import styles from './css/confirm.module.css';

export class ConfirmDialog extends Component {
    static OK = 0;
    static YesNo = 1;

    static ResultOK = 0;
    static ResultYes = 1;
    static ResultNo = 2;

    static Left = 0;
    static Center = 1;
    static Right = 2;

    static WarningTag = "[warning]";

    static getOK() {
        return ConfirmDialog.OK;
    }

    static getYesNo() {
        return ConfirmDialog.YesNo;
    }

    static getResultOK() {
        return ConfirmDialog.OK;
    }

    static getResultYes() {
        return ConfirmDialog.ResultYes;
    }

    static getResultNo() {
        return ConfirmDialog.ResultNo;
    }

    static getWarningTag() {
        return ConfirmDialog.WarningTag;
    }

    static getHeight(messageCount) {
        let height = 0;

        if (messageCount === 0) {
            height = 50;
        }
        else if (messageCount <= 3) {
            height = 60 + 30 * messageCount;
        }
        else {
            height = 65 + 30 * messageCount;
        }

        return height.toString() + "px";
    }

    onClickButton(e, option) {
        if (e.target) {
            const children = e.target.parentElement.children;
            const childCount = children.length;

            for (let i = 0; i < childCount; i++) {
                const btn = children[i];

                if (btn.tagName === "BUTTON") {
                    btn.setAttribute("disabled", true);
                }
            }

            this.props.onClickConfirm(option);
        }
    }

    getButtons() {
        let buttonAreaClassName = styles.buttonArea;

        if (this.props.buttonPosition) {
            if (this.props.buttonPosition === ConfirmDialog.Center) {
                buttonAreaClassName += " " + styles.center;
            }
            else if (this.props.buttonPosition === ConfirmDialog.Right) {
                buttonAreaClassName += " " + styles.right;
            }
        }

        if (this.props.option === ConfirmDialog.YesNo) {
            return (
                <div className={buttonAreaClassName}>
                    <button className={styles.btn} onClick={(e) => this.onClickButton(e, ConfirmDialog.ResultYes)}>예</button>
                    <button className={styles.btn} onClick={(e) => this.onClickButton(e, ConfirmDialog.ResultNo)}>아니오</button>
                </div>
            );
        }
        else if (this.props.option === ConfirmDialog.OK) {
            return (
                <div className={buttonAreaClassName}>
                    <button className={styles.btn} onClick={(e) => this.onClickButton(e, ConfirmDialog.ResultOK)}>확인</button>
                </div>
            );
        }

        return (
            <div className={styles.buttonArea}>
            </div>
        );
    }

    getMessageClassName(message) {
        if (message.startsWith(ConfirmDialog.WarningTag)) {
            return styles.message + " " + styles.warning;
        }

        return styles.message;
    }

    removeWarning(message) {
        if (message.startsWith(ConfirmDialog.WarningTag)) {
            return message.substring(ConfirmDialog.WarningTag.length);
        }

        return message;
    }

    render() {
        if (this.props.messages === null) {
            return <></>;
        }

        const buttons = this.getButtons();
        let index = 0; 
        
        return (
            <div className={styles.contentsArea}>
                {
                    this.props.messages.map(message => (
                        <span key={index++} className={this.getMessageClassName(message)}>{this.removeWarning(message)}</span>

                    ))
                }
                {buttons} 
            </div>
        );
    }
}