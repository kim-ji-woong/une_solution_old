import React, { Component } from 'react';
import Arrow from './arrow';
import sectionStyles from '../../css/section.module.css';
import $ from 'jquery';

const PositionType = { Top : 0, Right : 1, Bottom : 2, Left : 3, None : 4 };

class ArrowButton extends Component {
    constructor(props) {
        super(props);

        this.props = props;
        this.refButton = React.createRef();

        this.state =
        {
            positionType: this.props.positionType,
            instance: this,
            prevProps: this.props
        };
    }

    componentDidMount() {
        // 실행모드일 경우 컴포넌트 화살표 안보이게 처리
        if (this.props.mode === "exec") {
            const className = this.getClassName();

            $('.' + className).css('background-image', 'none');
            $('.' + className + ':hover').css('background-image', 'none');
        }
    }

    getClassName()
    {
        if (this.state.positionType === Arrow.Top)
        {
            return sectionStyles.btnArrowTop;
        }
        else if (this.state.positionType === Arrow.Bottom)
        {
            return sectionStyles.btnArrowBottom;
        }
        else if (this.state.positionType === Arrow.Left)
        {
            return sectionStyles.btnArrowLeft;
        }
        else if (this.state.positionType === Arrow.Right)
        {
            return sectionStyles.btnArrowRight;
        }

        return "";
    }

    onClickButton = () =>
    {
        this.props.onClickArrowButton(this.refButton.current, this.state.positionType);
    }

    render() {
        const className = this.getClassName();

        return (
            <div ref={this.refButton} className={className} onClick={this.onClickButton}>
            </div>
        );
    }
}

export default ArrowButton;