//
// Copyright (c) 2018 Håkan Edling
//
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// 
// http://github.com/tidyui/razorblog
//

if (typeof(razorblog)  == 'undefined')
    razorblog = {};

razorblog.comments = new function() {
    'use strict';

    var self = this;

    self.load = function (postId, domId) {
        var xmlhttp = new XMLHttpRequest();

        xmlhttp.onreadystatechange = function() {
            if (xmlhttp.readyState == XMLHttpRequest.DONE) {
               if (xmlhttp.status == 200) {
                   document.getElementById(domId).innerHTML = xmlhttp.responseText;
               }
             }
        };
    
        xmlhttp.open('GET', '/comments/' + postId, true);
        xmlhttp.send();    
    };
};