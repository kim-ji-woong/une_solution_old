import React, { Component } from 'react';
import space from './../css/space.module.css';

export class ModelView extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: '',
        }

        this.refLayer = React.createRef();
        this.refScrollArea = React.createRef();
        this.refScrollbar = React.createRef();
        this.refTree = React.createRef();
        /* this.seekBarRef = React.createRef(); */
    }


    /* const Slider = () => {
       const slider = document.getElementById('myRange');
       var output = document.getElementById('demo');

       output.innerHTML = slider.value;

       slider.oninput = function () {
          output.innerHTML = this.value;
       }
    } */


    render() {
        return (
            <>
              <div className={space.viewArea}>
                <div className={space.viewBox}>
                    <div className={space.viewTitleBox}>
                        <span>뷰 포인트</span>
                        <span>42/65</span>
                        <span className={space.viewSelect}></span>
                    </div>
                    <div className={space.viewContent}>
                      <div><span className={space.viewText1}>1. A site</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                      <div><span className={space.viewText2}>1.1 A Building</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                      <div><span className={space.viewText3}>1.1.1 1F</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                      <div><span className={space.viewText4}>1.1.2 2F</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                      <div><span className={space.viewText5}>1.1.3 3F</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                      <div><span className={space.viewText6}>1.1.4 4F</span><span className={space.viewChangeIcon}></span><span className={space.viewIcon}></span></div>
                    </div>
                </div>
                <div className={space.preview}>
                    <div className={space.preTitle}>미리보기</div>
                    <div className={space.preContent}>
                        <span className={space.basicSet}>기본뷰로 설정</span>
                        <span className={space.lightSet} onclick={this.onclickSet}>조도 설정</span>
                     </div>

                    {/* <div class="slidecontainer" ref={this.seekBarRef}>
                        <input type="range" min="1" max="100" value="50" class="slider" id="myRange" />
                     </div> */}
                 </div>
               </div>
            </>
        );
    }
}