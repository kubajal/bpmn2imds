import { Component, OnInit } from '@angular/core';
import * as Modeler from "bpmn-js/dist/bpmn-modeler.production.min.js";

export enum Color {
  Black = 'black',
  Yellow = 'yellow',
  Red = 'red',
  Green = 'green',
  Blue = 'blue'
}

export enum Status {
  Unknown = '1',
  Deadlock = '2',
  NoDeadlock = '3',
  DiagramChanged = '4'
}

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  public modeler: any;
  private modeling: any;
  private elementRegistry: any;

  constructor() {
    this.modeler = new Modeler({
      width: '100%',
      height: '600px',
      propertiesPanel: {
        parent: '#properties'
      },
      keyboard: { bindTo: document }
    });
    this.modeler.importXML('<?xml version="1.0" encoding="UTF-8"?><bpmn:definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:bpmn="http://www.omg.org/spec/BPMN/20100524/MODEL" xmlns:bpmndi="http://www.omg.org/spec/BPMN/20100524/DI" id="Definitions_12isity" targetNamespace="http://bpmn.io/schema/bpmn" exporter="bpmn-js (https://demo.bpmn.io)" exporterVersion="3.4.0"><bpmn:process id="Process_00e2sh0" isExecutable="false" /><bpmndi:BPMNDiagram id="BPMNDiagram_1"><bpmndi:BPMNPlane id="BPMNPlane_1" bpmnElement="Process_00e2sh0" /></bpmndi:BPMNDiagram></bpmn:definitions>z');
    this.modeling = this.modeler.get('modeling');
    this.elementRegistry = this.modeler.get('elementRegistry');
    let editorService = this;
    this.modeler.get('eventBus').on('shape.remove', 999999, function (event) {
      if (event.element.type != "label") {
        // editorService.deadlockStatus = Status.DiagramChanged;
        // editorService.deadlocks = [];
        // editorService.activeDeadlock = null;
      }
    });
    this.modeler.get('eventBus').on('connection.remove', 999999, function (event) {
      if (event.element.type != "label") {
        // editorService.deadlockStatus = Status.DiagramChanged;
        // editorService.deadlocks = [];
        // editorService.activeDeadlock = null;
      }
    });
    this.modeler.get('eventBus').on('element.click', 999999, function (event) {
      // editorService.clickedElement = event.element.id;
    }); }

  ngOnInit(): void {
    this.modeler.attachTo('#modeler');
  }
}
