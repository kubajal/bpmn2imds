import { Component, OnInit } from '@angular/core';
import { TransformationRule } from "./classes/TransformationRule";
import { GeneralDocumentationEntry } from "./classes/GeneralDocumentationEntry";
import { MappingDocumentationEntry } from "./classes/MappingDocumentationEntry";
import generalDocs from "./config/docs.general.json";
import mappingDocs from "./config/docs.mapping.json";
import { MappingRow } from './classes/MappingRow';

@Component({
  selector: 'app-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss']
})
export class HelpComponent implements OnInit {

  normalizationRules: Array<TransformationRule> = null;
  generalDocEntries: Array<GeneralDocumentationEntry> = null;
  mappingDocEntries: Array<MappingDocumentationEntry> = null;

  constructor() {
    this.normalizationRules = [new TransformationRule("parallel gateway", "img/parallel.svg", "parallel-normalized.svg")]
    this.generalDocEntries = <Array<any>>generalDocs.map(e => new GeneralDocumentationEntry((<any>e).question, (<any>e).id, (<any>e).answer));
    this.mappingDocEntries = <Array<any>>mappingDocs.map(e => {
      let mappingRows = <Array<MappingRow>>e.mapping;
      return new MappingDocumentationEntry((<any>e).question, (<any>e).id, mappingRows);
   });
  }

  ngOnInit(): void {
  }

}
