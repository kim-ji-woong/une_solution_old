import { Button } from '@amcharts/amcharts4/core';
import React, { Component } from 'react';
import newStyles from "../../../Common/css/newStyle.module.css";
import ConfirmDialog from '../../../Common/ui/confirmDialog';
import HistoryController from '../../services/historyController';

class SensorDetectHistoryMemo extends Component {
	constructor(props) {
		super(props);
		this.state = {
			displayMemo: this.props.popupMemoContent,

			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
        }
		this.props = props;
	}

	onChangeMemo = (e) => {
		this.setState({ displayMemo: e.target.value });
    }

	onSave = async () => {
		const result = await HistoryController.UpdateAlarmMemo(this.props.actionStepHistoryID, this.state.displayMemo);
		if (result) {
			this.props.setPopupMemo(false, this.props.actionStepHistoryID, this.state.displayMemo);
		}
		else {
			this.showConfirmDialog("오류", '메모를 저장 할 수 없습니다.', null, null);
        }
	}

	showConfirmDialog = (title, messages, buttons, onClickButton) => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = true;
		confirmMessage.title = title;
		confirmMessage.buttons = buttons;
		confirmMessage.onClickButton = onClickButton;

		if (!messages) {
			confirmMessage.messages = [""];
		}
		else if (Array.isArray(messages)) {
			confirmMessage.messages = messages;
		}
		else {
			confirmMessage.messages = [messages];
		}

		this.setState({ confirmMessage });
	}

	onCloseConfirmDialog = () => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = false;

		this.setState({ confirmMessage });
	}

	render() {
		return (
			<>
				<div id={newStyles.hsMmo} className={newStyles.popup}>
					<div>
						<div>
							<div className={newStyles.hsmCont}>
								<div className={newStyles.hsmTitle}>
									<h3>메모</h3>
									<a className={newStyles.hsmCls} onClick={() => this.props.setPopupMemo(-1)}>닫기</a>
								</div>
								<textarea name="" id="" cols="30" rows="10" className={"scroll-wrapper" + newStyles.hsmTxt + "scrollbar scroll-textarea"} onChange={(e) => this.onChangeMemo(e)}>
									{this.state.displayMemo}
								</textarea>
								<ul className={newStyles.hsmBtn}>
									<li><a onClick={() => this.props.setPopupMemo(false)}>취소</a></li>
									<li><a onClick={() => this.onSave()}>저장</a></li>
								</ul>
							</div>
						</div>
					</div>
				</div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</>
		);
	}
}

export default SensorDetectHistoryMemo;