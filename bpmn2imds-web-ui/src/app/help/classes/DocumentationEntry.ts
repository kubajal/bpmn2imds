/*
 *  Question-answer pair.
 */
export class DocumentationEntry {

  constructor(q: string, i: string) {
    this.question = q;
    this.id = i;
  }
  id: string;
  question: string;
}