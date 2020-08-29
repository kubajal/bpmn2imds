import { DocumentationEntry } from "./DocumentationEntry";
import { MappingRow } from "./MappingRow";

/*
 *  Question-answer pair.
 */
export class MappingDocumentationEntry extends DocumentationEntry {

  constructor(q: string, i: string, m: Array<MappingRow>) {
    super(q, i);
    this.mapping = m;
  }

  mapping: Array<MappingRow> = null;
}