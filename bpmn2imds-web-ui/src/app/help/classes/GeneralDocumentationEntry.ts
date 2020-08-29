import { DocumentationEntry } from "./DocumentationEntry";

/*
 *  Question-answer pair.
 */
export class GeneralDocumentationEntry extends DocumentationEntry {

  constructor(q: string, i:string, a: Array<string>) {
    super(q, i);
    this.answer = a;
  }
  answer: Array<string>;
}