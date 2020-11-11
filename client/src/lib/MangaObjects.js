"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MangaChapter = exports.MangaPage = void 0;
var MangaPage = /** @class */ (function () {
    function MangaPage() {
        this.isDouble = false;
    }
    return MangaPage;
}());
exports.MangaPage = MangaPage;
var MangaChapter = /** @class */ (function () {
    function MangaChapter() {
        this.name = "<New Chapter>";
        this.rtl = true;
        this.pages = [];
    }
    MangaChapter.mockChapter = function (name, rtl, pageCount) {
        var result = new MangaChapter();
        result.name = name;
        result.rtl = rtl;
        result.pages = [];
        for (var i = 0; i < pageCount; i++) {
            result.pages.push(new MangaPage);
        }
        return result;
    };
    return MangaChapter;
}());
exports.MangaChapter = MangaChapter;
