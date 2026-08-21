import React, { Component } from 'react';
import styles from '../../../Common/css/style.module.css';
import bodyStyles from '../../css/body.module.css';
import '../../css/componentProperty.css';
import '../../../Common/css/scroll.css';
import commonStyles from '../../../Common/css/common.module.css';
import SectionDataDecision from '../../../Common/models/sections/sectionDataDecision';

class DecisionProperty extends Component {
	static cssStyles = styles;

	constructor(props) {
		super(props);
		this.props = props;

		const sectionData = new SectionDataDecision();

		if (this.props.sectionData) {
			SectionDataDecision.copyTo(this.props.sectionData, sectionData);
		}

		this.state = {
			instance: this,
			sectionData: sectionData,
			useVariables: false,
			variableOn: false,
			descriptionOn: false,
			prevProps: this.props
		}

		this.refTitle = React.createRef();
		this.refVariableDT = React.createRef();
		this.refVariableDD = React.createRef();
		this.refDescriptionDT = React.createRef();
		this.refDescriptionDD = React.createRef();
		this.refDescription = React.createRef();
	}

	componentWillUnmount() {
		// 창이 닫히게 될 경우 편집한 내용을 저장한다.
		this.saveSectionData(false);
	}

	static getDerivedStateFromProps(props, state) {
		if (props === state.prevProps) {
			return state;
		}

		if (props.sectionData !== state.prevProps.sectionData && state.sectionData) {
			// 다른 Process를 선택하여 창이 바뀌게 될 경우 편집한 내용을 저장한다.
			state.instance.saveSectionData(false);
		}

		const sectionData = new SectionDataDecision();

		if (props.sectionData) {
			SectionDataDecision.copyTo(props.sectionData, sectionData);
		}

		state.instance.refTitle.current.value = state.instance.refTitle.current.text = sectionData.text;

		return {
			instance: state.instance,
			sectionData: sectionData,
			useVariables: state.useVariables,
			variableOn: state.variableOn,
			descriptionOn: state.descriptionOn,
			prevProps: props
		};
	}

	onClickToggle = (event) => {
		if (event.target.classList.contains(DecisionProperty.cssStyles.on)) {
			event.target.classList.remove(DecisionProperty.cssStyles.on);
		}
		else {
			event.target.classList.add(DecisionProperty.cssStyles.on);
		}
	}

	onTextChange = (event) => {
	}

	onDescriptionChange = (event) => {
		if (this.state.sectionData) {
			const sectionData = { ...this.state.sectionData };
			sectionData.description = event.target.value;
			this.setState({ sectionData });
        }
    }

	onCheckUseVariables = (event) => {
		if (event.target.checked) {
			if (this.refVariableDT.current.classList.contains(DecisionProperty.cssStyles.on) === false) {
				this.refVariableDT.current.classList.add(DecisionProperty.cssStyles.on);
			}

			if (this.refVariableDD.current.classList.contains(DecisionProperty.cssStyles.on) === false) {
				this.refVariableDD.current.classList.add(DecisionProperty.cssStyles.on);
			}
		}
		else {
			if (this.refVariableDT.current.classList.contains(DecisionProperty.cssStyles.on)) {
				this.refVariableDT.current.classList.remove(DecisionProperty.cssStyles.on);
			}

			if (this.refVariableDD.current.classList.contains(DecisionProperty.cssStyles.on)) {
				this.refVariableDD.current.classList.remove(DecisionProperty.cssStyles.on);
			}
        }

		this.setState({ useVariables: event.target.checked });
	}

	getVariableClassName() {
		if (this.state.variableOn) {
			return DecisionProperty.cssStyles.on;
		}

		return "";
	}

	getDescriptionClassName() {
		if (this.state.descriptionOn) {
			return DecisionProperty.cssStyles.on;
		}

		return "";
	}

	onClickCascade = (event) => {
		const element = event.target;

		if (event.target.classList.contains(DecisionProperty.cssStyles.on)) {
			event.target.classList.remove(DecisionProperty.cssStyles.on);

			if (element === this.refVariableDT.current) {
				this.refVariableDT.current.classList.remove(DecisionProperty.cssStyles.on);

				this.setState({ variableOn: false });
			}
			else if (element === this.refDescriptionDT.current) {
				this.refDescriptionDT.current.classList.remove(DecisionProperty.cssStyles.on);

				this.setState({ descriptionOn: false });
            }
		}
		else {
			event.target.classList.add(DecisionProperty.cssStyles.on);

			if (element === this.refVariableDT.current) {
				if (this.refVariableDT.current.classList.contains(DecisionProperty.cssStyles.on) === false) {
					this.refVariableDT.current.classList.add(DecisionProperty.cssStyles.on);
				}

				this.refDescriptionDT.current.classList.remove(DecisionProperty.cssStyles.on);

				this.setState({ variableOn: true, descriptionOn: false });
			}
			else if (element === this.refDescriptionDT.current) {
				if (this.refDescriptionDT.current.classList.contains(DecisionProperty.cssStyles.on) === false) {
					this.refDescriptionDT.current.classList.add(DecisionProperty.cssStyles.on);
				}

				this.refVariableDT.current.classList.remove(DecisionProperty.cssStyles.on);

				this.setState({ variableOn: false, descriptionOn: true });
			}
        }
	}

	onClickApply(ok) {
		if (ok) {
			this.saveSectionData(true);
		}
		else {
			this.setState({
				sectionData: null,
				useVariables: false
			});
		}
	}

	saveSectionData(shouldUpdate) {
		const sectionData = { ...this.state.sectionData };
		sectionData.text = this.refTitle.current.value;
		sectionData.description = this.refDescription.current.value;

		this.props.onApplyComponentProperty(sectionData, this.props.actionStep, shouldUpdate);
	}

	render() {
		const description = this.state.sectionData?.description ? this.state.sectionData.description : "";

        return (
			<div className={DecisionProperty.cssStyles.sprCont + " " + DecisionProperty.cssStyles.pt150 + " " + bodyStyles.noDrag}>
				<div className={DecisionProperty.cssStyles.sprTop}>
					<h4 className={DecisionProperty.cssStyles.sprtTitle}>판단문 작성</h4>
					<div className={"scroll-wrapper " + DecisionProperty.cssStyles.sprtTxtara + " " + DecisionProperty.cssStyles.ssk + " scrollbar-outer scroll-textarea"} id="pos_relative">
						<div className="scroll-content scroll-scrolly_visible" id="decision_scrollContent">
							<textarea ref={this.refTitle} name="" id="" cols="30" rows="10" className={DecisionProperty.cssStyles.sprtTxtara + " " + DecisionProperty.cssStyles.ssk + " scrollbar-outer"} defaultValue={this.state.sectionData?.text} onChange={this.onTextChange}></textarea>
						</div>
						<div className="scroll-element scroll-x scroll-scrolly_visible"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="width_88px"></div></div></div>
						<div className="scroll-element scroll-y scroll-scrolly_visible"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="height_29px_top0"></div></div></div>
					</div>
					<div className={DecisionProperty.cssStyles.sskChk}>
						<label className={DecisionProperty.cssStyles.clickable}>
							<input type="checkbox" name="sskChk" className={bodyStyles.labelInput} id={DecisionProperty.cssStyles.sskChk} checked={this.state.useVariables} onChange={this.onCheckUseVariables}/>
							수식사용
						</label>
					</div>
				</div>
				<div className={DecisionProperty.cssStyles.sprMid}>
					<div className="scroll-wrapper scrollbar-outer" id="pos_relative">
						<div className="scrollbar-outer scroll-content" id="decision_scrollContent2">
							<div className={DecisionProperty.cssStyles.sprmCont}>
								<dl className={DecisionProperty.cssStyles.sprmAcdn}>
									<dt ref={this.refVariableDT} className={this.getVariableClassName()} onClick={this.onClickCascade}>수식</dt>
									<dd ref={this.refVariableDD} className={this.getVariableClassName()}>
										<div className={"scroll-wrapper " + DecisionProperty.cssStyles.sprtTxtara + " scrollbar-outer scroll-textarea"} id="pos_relative"><div className="scroll-content scroll-scrolly_visible" id="decision_scrollContent3"><textarea name="" id="" cols="30" rows="10" className={DecisionProperty.cssStyles.sprtTxtara + " scrollbar-outer"}></textarea></div><div className="scroll-element scroll-x scroll-scrolly_visible"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="width_89px"></div></div></div><div className="scroll-element scroll-y scroll-scrolly_visible"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="height_65px_top0"></div></div></div></div>
										<h5>기본타입</h5>
										<div className={DecisionProperty.cssStyles.sopEdtTb}>
											<table>
												<caption>변수, 타입, 설명으로 구성된 표</caption>
												<colgroup>
													<col id="width_25Pro" />
													<col id="width_25Pro" />
													<col id="width_50Pro" />
												</colgroup>
												<thead>
													<tr>
														<th>변수</th>
														<th>Type</th>
														<th>설명</th>
													</tr>
												</thead>
												<tbody>
													<tr onClick={this.onClickToggle}>
														<td>time</td>
														<td>문자열</td>
														<td className={commonStyles.tal}><p className={commonStyles.nwrp}>재난발생 시간 재난발생 시간 재난발생 시간</p></td>
													</tr>
													<tr onClick={this.onClickToggle}>
														<td>location</td>
														<td>문자열</td>
														<td className={commonStyles.tal}><p className={commonStyles.nwrp}>재난발생 위치 재난발생 위치 재난발생 위치</p></td>
													</tr>
													<tr>
														<td>-</td>
														<td>-</td>
														<td>-</td>
													</tr>
												</tbody>
											</table>
										</div>
										<h5>사용자 정의 타입</h5>
										<div className={DecisionProperty.cssStyles.sopEdtTb}>
											<table>
												<caption>변수, 타입, 설명으로 구성된 표</caption>
												<colgroup>
													<col id="width_25Pro" />
													<col id="width_25Pro" />
													<col id="width_50Pro" />
												</colgroup>
												<thead>
													<tr>
														<th>변수</th>
														<th>Type</th>
														<th>설명</th>
													</tr>
												</thead>
												<tbody>
													<tr onClick={this.onClickToggle}>
														<td>time</td>
														<td>문자열</td>
														<td className={commonStyles.tal}><p className={commonStyles.nwrp}>재난발생 시간 재난발생 시간 재난발생 시간</p></td>
													</tr>
													<tr onClick={this.onClickToggle}>
														<td>location</td>
														<td>문자열</td>
														<td className={commonStyles.tal}><p className={commonStyles.nwrp}>재난발생 위치 재난발생 위치 재난발생 위치</p></td>
													</tr>
													<tr>
														<td>-</td>
														<td>-</td>
													</tr>
												</tbody>
											</table>
										</div>
									</dd>
									<dt ref={this.refDescriptionDT} className={this.getDescriptionClassName()} onClick={this.onClickCascade}>판단 기준 설명</dt>
									<dd ref={this.refDescriptionDD} className={this.getDescriptionClassName()}>
										<div className={"scroll-wrapper " + DecisionProperty.cssStyles.sprtTxtara + " scrollbar-outer scroll-textarea"} id="pos_relative">
											<div className="scroll-content scroll-scrolly_visible" id="decision_scrollContent3">
												<textarea ref={this.refDescription} cols="30" rows="10" className={DecisionProperty.cssStyles.sprtTxtara + " scrollbar-outer"} value={description} onChange={this.onDescriptionChange}></textarea>
											</div>
											<div className="scroll-element scroll-x scroll-scrolly_visible">
												<div className="scroll-element_outer">
													<div className="scroll-element_size"></div>
													<div className="scroll-element_track"></div>
													<div className="scroll-bar" id="width_89px"></div>
												</div>
											</div>
											<div className="scroll-element scroll-y scroll-scrolly_visible">
												<div className="scroll-element_outer">
													<div className="scroll-element_size"></div>
													<div className="scroll-element_track"></div>
													<div className="scroll-bar" id="height_65px_top0"></div>
												</div>
											</div>
										</div>
									</dd>
								</dl>
							</div>
						</div><div className="scroll-element scroll-x"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="width_100px"></div></div></div><div className="scroll-element scroll-y"><div className="scroll-element_outer"><div className="scroll-element_size"></div><div className="scroll-element_track"></div><div className="scroll-bar" id="height_100px"></div></div></div>
					</div>
				</div>
				<div className={DecisionProperty.cssStyles.sprBot}>
					<a className={DecisionProperty.cssStyles.clickable} onClick={() => this.onClickApply(true)}>확인</a>
					<a className={DecisionProperty.cssStyles.clickable} onClick={() => this.onClickApply(false)}>취소</a>
				</div>
			</div>
            );
    }
    /*constructor(props) {
        super(props);
        this.props = props;

        this.state = {
            sectionData: this.props.sectionData
        }

        this.refText = React.createRef();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.sectionData !== nextProps.sectionData) {
            this.setState({ sectionData: nextProps.sectionData });
        }
    }

    onApplyComponentProperty = () => {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = this.refText.current.value;
        this.props.onApplyComponentProperty(sectionData, this.props.actionStep);
    }

    onChangeText = (event) => {
        const sectionData = { ...this.state.sectionData };
        sectionData.text = this.refText.current.value;
        this.setState({ sectionData });
    }

    render() {
        return (
            <>
                <div className="componentProperties">
                    <span className="componentType">판단</span>
                    <div className="decisionProperty">
                        <label className="decisionLabel">내용</label>
                        <textarea ref={this.refText} className="componentText" value={this.state.sectionData.text} onChange={this.onChangeText}/>
                    </div>
                </div>
                <button className="btnApply" onClick={this.onApplyComponentProperty}>적용</button>
            </>
        );
    }*/
}

export default DecisionProperty;