import React, { Component } from 'react';
import styles from '../css/modal.module.css';

export class ConfirmDialog extends Component {
    static keys = [];
    static idxEnter = -1;       // Enter 단축키 인덱스 번호 

	constructor(props) {
        super(props);

        this.refBody = React.createRef();
        this.clickX = 0;
        this.clickY = 0;
        this.moveX = 0;
        this.moveY = 0;
        this.originMoveX = 0;
        this.originMoveY = 0;
    }

    componentDidMount() {
        const body = this.refBody.current;

        if (!body) {
            return;
        }

        // 팝업 마우스 드래그 이벤트 리스너
        this.popupDragMouseMove = (event) => {
            this.moveX = event.clientX - this.clickX + this.originMoveX;
            this.moveY = event.clientY - this.clickY + this.originMoveY;

            body.style.transform = `translate(${this.moveX}px, ${this.moveY}px)`;
        }

        // 단축키 이벤트 리스너
        document.addEventListener("keydown", this.keyFunction, false);
        document.addEventListener("keyup", this.keysReleased, false);
    }

    componentWillUnmount() {
        // 단축키 이벤트 리스너 제거
        document.removeEventListener("keydown", this.keyFunction);
        document.removeEventListener("keyup", this.keysReleased);
    }

    // 팝업 드래그 시작(팝업을 누르고 있을 때)
    popupDragMousePress(event) {
        if (event.button == 0) {
            this.clickX = event.clientX;
            this.clickY = event.clientY;
            
            document.addEventListener('mousemove', this.popupDragMouseMove);
            document.addEventListener('mouseup', this.popupDragMouseUp);
        }
    }

    // 팝업 드래그 종료(mouse up)
    popupDragMouseUp = () => {
        document.removeEventListener('mousemove', this.popupDragMouseMove);
        document.removeEventListener('mouseup', this.popupDragMouseUp);

        this.originMoveX = this.moveX;
        this.originMoveY = this.moveY;
    }

    keyFunction = (e) => this.keysPressed(e, this);

    keysPressed(e, target) {
        // store an entry for every key pressed
        ConfirmDialog.keys[e.keyCode] = true;

        if (ConfirmDialog.keys[27]) {
            // ESC 누를 시 
            target.props.onClose();

            ConfirmDialog.keys[27] = false;
            // prevent default browser behavior
            e.preventDefault();
        } else if (ConfirmDialog.keys[13]) {
            // Enter 누를 시 
            target.props.onClickButton(ConfirmDialog.idxEnter);

            ConfirmDialog.keys[13] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
    }

    keysReleased(e) {
        // mark keys that were released
        ConfirmDialog.keys[e.keyCode] = false;
    }

    getMessage() {
        const messages = [];

        this.props.messages.map((message, index) => {
            messages.push(
                <p key={"message_" + index}>{message}</p>
            );
        });

        return (
            <main>
                {messages}
            </main>
            );
    }

    getButtons() {
        const buttons = [];

        if (!this.props.buttons || this.props.buttons.length === 0) {
            buttons.push(
                <button key={"button_0"} className={styles.close} onClick={() => this.props.onClose()}>확인</button>
            );
        }
        else {
            this.props.buttons.map((button, index) => {
                if (this.props.onClickButton) {
                    if (button === "오작동") {
                        ConfirmDialog.idxEnter = index;

                        buttons.push(
                            <button key={"button_" + index} className={styles.close} onClick={() => this.props.onClickButton(index)} title="단축키(Enter)">{button}</button>
                        );
                    } else {
                        buttons.push(
                            <button key={"button_" + index} className={styles.close} onClick={() => this.props.onClickButton(index)}>{button}</button>
                        );
                    }
                    
                    
                }
                else {
                    buttons.push(
                        <button key={"button_" + index} className={styles.close} onClick={this.props.onClose}>{button}</button>
                    );
                }
            });
        }

        return (
            <footer>
                {buttons}
            </footer>
            );
    }

    render() {
        if (!this.props.onClose) {
            return <></>
        }

		return (
            <div ref={this.refBody} className={styles.modal + " " + styles.openModal}>
                <section>
                    <header onMouseDown={(e) => this.popupDragMousePress(e)}>
                        {this.props.title}
                        <button className={styles.close} onClick={this.props.onClose}> &times; </button>
                    </header>
                    {
                        this.getMessage()
                    }
                    {
                        this.getButtons()
                    }
                </section>
            </div>
			);
    }
}

export default ConfirmDialog;