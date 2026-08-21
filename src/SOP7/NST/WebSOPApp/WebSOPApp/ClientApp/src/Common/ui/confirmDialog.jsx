import * as React from 'react';
import styles from '../css/modal.module.css';

/*interface Props {
    title: string,
    messages: Array<string>,
    buttons: Array<string> | null,
    onClose: () => void,
    onClickButton: (index: number) => void
}*/

export class ConfirmDialog extends React.Component/*<Props>*/ {
    /*private refBody: React.RefObject<HTMLDivElement>;
    private clickX: number;
    private clickY: number;
    private moveX: number;
    private moveY: number;
    private originMoveX: number;
    private originMoveY: number;

    private popupDragMouseMove: (event: MouseEvent) => {};*/

    constructor(props/*: Props*/) {
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
        this.popupDragMouseMove = (event/*: MouseEvent*/) => {
            this.moveX = event.clientX - this.clickX + this.originMoveX;
            this.moveY = event.clientY - this.clickY + this.originMoveY;

            body.style.transform = `translate(${this.moveX}px, ${this.moveY}px)`;
        }
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
                    buttons.push(
                        <button key={"button_" + index} className={styles.close} onClick={() => this.props.onClickButton(index)}>{button}</button>
                    );
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