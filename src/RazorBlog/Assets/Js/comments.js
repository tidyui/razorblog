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

    // Adds a new comment
    self.add = function (comment, onSuccess, onError) {
        // Validate input
        if (comment.PostId == '' || comment.Body == '' || comment.AuthorName == '' || comment.AuthorEmail == '') {
            if (onError != null)
                onError();
        }

        // Create xml request
        var xmlhttp = new XMLHttpRequest();
        xmlhttp.onreadystatechange = function() {
            if (xmlhttp.readyState == XMLHttpRequest.DONE) {
                if (xmlhttp.status == 200) {
                    if (onSuccess != null)
                        onSuccess();
                } else {
                    if (onError != null)
                        onError();
                }
            }
        };
        
        // Call api
        xmlhttp.open('POST', '/razorblog/api/comment', true);
        xmlhttp.setRequestHeader('Content-Type', 'application/json');
        xmlhttp.send(JSON.stringify(comment));
    };

    // Loads comments for a post into the given dom element.
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

    // Loads the specified page of comments for a post into the given dom element.    
    self.loadPage = function (postId, domId, page) {
        var xmlhttp = new XMLHttpRequest();

        xmlhttp.onreadystatechange = function() {
            if (xmlhttp.readyState == XMLHttpRequest.DONE) {
               if (xmlhttp.status == 200) {
                   document.getElementById(domId).innerHTML = xmlhttp.responseText;
               }
            }
        };
    
        xmlhttp.open('GET', '/comments/' + postId + '/' + page, true);
        xmlhttp.send();    
    };

    self.validate = function ()
    {
        var isValid = true;

        var body = document.getElementById('Body');
        if (body.value == '') {
            isValid = false;
            body.classList.add('error');
        } else {
            body.classList.remove('error');
        }

        var author = document.getElementById('AuthorName');
        if (author.value == '') {
            isValid = false;
            author.classList.add('error');
        } else {
            author.classList.remove('error');
        }

        var email = document.getElementById('AuthorEmail');
        if (email.value == '') {
            isValid = false;
            email.classList.add('error');
        } else {
            email.classList.remove('error');
        }

        if (!isValid) {
            return false;
        } else {
            return true;
        }
    };
};