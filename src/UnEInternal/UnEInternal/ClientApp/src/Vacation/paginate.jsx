import React, { Component } from 'react';
import styles from './css/paginate.module.css';

class Paginate extends Component {
    constructor(props) {
        super(props);

        this.state =
        {
            page: null,
            allRequest: null,

            ongPage: 10,                 // 한 페이지에 보여줄 요청의 수.
            oneSection: 10,             // 한번에 보여줄 총 페이지 수
            allPage: null,              // 전체 페이지 수
        };

        this.props = props;

        this.state.page = this.props.page;
        this.state.allRequest = this.props.allRequest;
        this.state.allPage = Math.ceil(this.state.allRequest / this.state.ongPage);

    }

    componentWillUpdate(nextProps, nextState) {
        //console.log('componentWillUpdate');
    }

    componentDidUpdate(prevProps, prevState) {
        //console.log('componentDidUpdate');

        if (this.props.allRequest !== prevProps.allRequest) {
            this.state.page = this.props.page;
            this.state.allRequest = this.props.allRequest;
            this.state.allPage = Math.ceil(this.state.allRequest / this.state.ongPage);

            this.setState({ page: this.state.page, allRequest: this.state.allRequest, allPage: this.state.allPage});
        }
    }

    onClickPage = (pageNum) => {
        this.setState({ page: pageNum });
        this.props.onChange(pageNum);
        return;
    }

    onClickPre = () => {
        const pageNumPre = this.state.page - 1;
        this.setState({ page: pageNumPre });
        this.props.onChange(pageNumPre);
        return;
    }

    onClickNext = () => {
        const pageNumNext = this.state.page + 1;
        this.setState({ page: pageNumNext });
        this.props.onChange(pageNumNext);
        return;
    }

    render() {
        if (this.state.allPage === 0 || this.state.allPage === 1 || this.state.page === null) {
            return (<> </>);
        }

        const paging = [];

        let first = 0;            // 페이징 표시 첫 페이지
        let last = 0;             // 페이징 표시 마지막 페이지

        // 페이징 첫 페이지 및 마지막 페이지 구하기
        if (this.state.page < Math.ceil(this.state.oneSection / 2) + 1 || this.state.allPage < this.state.oneSection) {
            first = 1;
            last = first + this.state.oneSection - 1;
        } else if (this.state.allPage - Math.floor(this.state.oneSection / 2) < this.state.page) {
            last = this.state.allPage;
            first = last - this.state.oneSection + 1;

            if (first === 0)
                first = 1;
        } else {
            first = this.state.page - Math.floor(this.state.oneSection / 2);
            last = this.state.page + Math.floor(this.state.oneSection / 2);

            if ((this.state.oneSection % 2) === 0) {
                last--;
            }
        }

        if (last > this.state.allPage) {
            last = this.state.allPage;
        }

        // 페이징 UI 생성
        if (this.state.page !== 1) {
            paging.push(<a key={Math.random()} className={styles.page} onClick={this.onClickPre} >이전</a>);
        }

        for (let i = first; i <= last; i++) {
            if (i === this.state.page) {
                paging.push(<a key={Math.random()} className={styles.pageNow}> {i} </a>);
            } else if (i !== this.state.page) {
                paging.push(<a key={Math.random()} className={styles.page} onClick={() => this.onClickPage(i)}> {i} </a>);
            }
        }

        if (this.state.page !== this.state.allPage) {
            paging.push(<a key={Math.random()} className={styles.page} onClick={this.onClickNext}>다음</a>);
        }

        return (
            <div className={styles.paginate}>
                {paging}
            </div>
        );
    }
}

export default Paginate;