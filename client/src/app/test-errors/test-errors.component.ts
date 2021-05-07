import { HttpClient } from '@angular/common/http';
import { collectExternalReferences } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {

  baseUrl = 'http://localhost:5000/api/';
  error: string = "";

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }
  get401Error() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe(respone => {
      console.log(respone)
    }, error => {
      this.error = this.formatJSON(JSON.stringify(error), true);
      console.log(error)
    })
  }

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe(respone => {
      console.log(respone)
    }, error => {
      this.error = this.formatJSON(JSON.stringify(error), true);
    })
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe(respone => {
      console.log(respone)
    }, error => {
      this.error = this.formatJSON(JSON.stringify(error), true);
    })
  }
  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe(respone => {
      console.log(respone)
    }, error => {
      this.error = this.formatJSON(JSON.stringify(error), true);
    })
  }
  get400ValidationError(){
    this.http.post(this.baseUrl + 'account/register', {}).subscribe(respone => {
      console.log(respone)
    }, error => {
      this.error = this.formatJSON(JSON.stringify(error), true);
    })
  }
  formatJSON(json,textarea) {
    var nl;
    if(textarea) {
        nl = "&#13;&#10;";
    } else {
        nl = "<br>";
    }
    var tab = "&#160;&#160;&#160;&#160;";
    var ret = "";
    var numquotes = 0;
    var betweenquotes = false;
    var firstquote = false;
    for (var i = 0; i < json.length; i++) {
        var c = json[i];
        if(c == '"') {
            numquotes ++;
            if((numquotes + 2) % 2 == 1) {
                betweenquotes = true;
            } else {
                betweenquotes = false;
            }
            if((numquotes + 3) % 4 == 0) {
                firstquote = true;
            } else {
                firstquote = false;
            }
        }

        if(c == '[' && !betweenquotes) {
            ret += c;
            ret += nl;
            continue;
        }
        if(c == '{' && !betweenquotes) {
            ret += tab;
            ret += c;
            ret += nl;
            continue;
        }
        if(c == '"' && firstquote) {
            ret += tab + tab;
            ret += c;
            continue;
        } else if (c == '"' && !firstquote) {
            ret += c;
            continue;
        }
        if(c == ',' && !betweenquotes) {
            ret += c;
            ret += nl;
            continue;
        }
        if(c == '}' && !betweenquotes) {
            ret += nl;
            ret += tab;
            ret += c;
            continue;
        }
        if(c == ']' && !betweenquotes) {
            ret += nl;
            ret += c;
            continue;
        }
        ret += c;
    } // i loop
    return ret;
}
}
