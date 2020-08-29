/*
 *  Question-answer pair.
 */
export class MappingRow {

  constructor(n: string, f: string, t: string) {
    this.name = n;
    this.from = f;
    this.to = t;
  }
  name: string;
  from: string;
  to: string;
}