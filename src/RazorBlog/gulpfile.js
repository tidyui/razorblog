var gulp = require('gulp'),
    rename = require("gulp-rename"),
    uglify = require("gulp-uglify");

var paths = {
    js: [
        "Assets/Js/comments.js"
    ]
};

gulp.task('min:js', function () {
    return gulp.src(paths.js, { base: "." })
        .pipe(uglify())
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(gulp.dest("."));
}); 