import React, { Component } from 'react';
import EndpointProperty from './endpointProperty';
import ProcessProperty from './processProperty';
import AnnotationProperty from './annotationProperty';
import DecisionProperty from './decisionProperty';
import InternalProperty from './internalProperty';
import SectionDataEndpoint from '../../../Common/models/sections/sectionDataEndpoint';
import SectionDataProcess from '../../../Common/models/sections/sectionDataProcess';
import SectionDataAnnotation from '../../../Common/models/sections/sectionDataAnnotation';
import SectionDataDecision from '../../../Common/models/sections/sectionDataDecision';
import SectionDataInternal from '../../../Common/models/sections/sectionDataInternal';
import SectionData from '../../../Common/models/sections/sectionData';
import SopManagerResource from '../../resource/id';
import styles from '../../../Common/css/style.module.css';
import ArrowProperty from './arrowProperty';

class ComponentProperties extends Component {
    static cssStyles = styles;

    getComponentProperty() {
        if (this.props.sectionData.componentType === SectionDataEndpoint.getComponentType() || this.props.sectionData.componentType === SectionData.EndpointType) {
            return <EndpointProperty sectionData={this.props.sectionData} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
        }
        else if (this.props.sectionData.componentType === SectionDataProcess.getComponentType() || this.props.sectionData.componentType === SectionData.ProcessType) {
            return <ProcessProperty sectionData={this.props.sectionData} teamAllTreeDatas={this.props.sopData.teamAllTreeDatas} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
        }
        else if (this.props.sectionData.componentType === SectionDataAnnotation.getComponentType() || this.props.sectionData.componentType === SectionData.AnnotationType) {
            return <AnnotationProperty sectionData={this.props.sectionData} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
        }
        else if (this.props.sectionData.componentType === SectionDataDecision.getComponentType() || this.props.sectionData.componentType === SectionData.DecisionType) {
            return <DecisionProperty sectionData={this.props.sectionData} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
        }
        else if (this.props.sectionData.componentType === SectionDataInternal.getComponentType() || this.props.sectionData.componentType === SectionData.InternalType) {
            return <InternalProperty sectionData={this.props.sectionData} teamAllTreeDatas={this.props.sopData.teamAllTreeDatas} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
        }
        return <></>
    }

    render() {
        if (this.props.sectionData === null || this.props.actionStep === null) {
            if (this.props.arrowData === null || this.props.actionStep === null) {
                return <h3 className={ComponentProperties.cssStyles.sprTitle}>{SopManagerResource.ID.componentProperty.title}</h3>
            }
            else {
                return (
                    <>
                        <h3 className={ComponentProperties.cssStyles.sprTitle}>{SopManagerResource.ID.componentProperty.title}</h3>
                        <ArrowProperty arrowData={this.props.arrowData} actionStep={this.props.actionStep} onApplyComponentProperty={this.props.onApplyComponentProperty} />
                    </>
                );
            }
        }

        return (
            <>
                <h3 className={ComponentProperties.cssStyles.sprTitle}>{SopManagerResource.ID.componentProperty.title}</h3>
                {
                    this.getComponentProperty()
                }
            </>
        );
    }
}

export default ComponentProperties;