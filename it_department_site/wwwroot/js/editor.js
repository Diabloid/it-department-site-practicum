import {
    ClassicEditor, Essentials, Paragraph, Heading, Autoformat, AutoLink, TextTransformation,
    Bold, Italic, Underline, Strikethrough, Subscript, Superscript, Code, RemoveFormat,
    FontFamily, FontSize, FontColor, FontBackgroundColor, Highlight, Alignment,
    Link, LinkImage, List, ListProperties, TodoList, Indent, IndentBlock,
    BlockQuote, CodeBlock, HorizontalLine, PageBreak, SpecialCharacters, SpecialCharactersEssentials,
    Table, TableToolbar, TableProperties, TableCellProperties, TableColumnResize, TableCaption,
    Image, ImageInsert, ImageUpload, ImageToolbar, ImageCaption, ImageStyle, ImageResize,
    MediaEmbed, HtmlEmbed, SourceEditing, ShowBlocks, FindAndReplace, PasteFromOffice,
    GeneralHtmlSupport, CKFinderUploadAdapter
} from 'ckeditor5';
import uk from 'ckeditor5/translations/uk.js';

export const fullConfig = {
    licenseKey: 'GPL',
    translations: [uk],
    plugins: [
        Essentials, Paragraph, Heading, Autoformat, AutoLink, TextTransformation,
        Bold, Italic, Underline, Strikethrough, Subscript, Superscript, Code, RemoveFormat,
        FontFamily, FontSize, FontColor, FontBackgroundColor, Highlight, Alignment,
        Link, LinkImage, List, ListProperties, TodoList, Indent, IndentBlock,
        BlockQuote, CodeBlock, HorizontalLine, PageBreak, SpecialCharacters, SpecialCharactersEssentials,
        Table, TableToolbar, TableProperties, TableCellProperties, TableColumnResize, TableCaption,
        Image, ImageInsert, ImageUpload, ImageToolbar, ImageCaption, ImageStyle, ImageResize,
        MediaEmbed, HtmlEmbed, SourceEditing, ShowBlocks, FindAndReplace, PasteFromOffice,
        GeneralHtmlSupport, CKFinderUploadAdapter
    ],
    toolbar: {
        items: [
            'undo', 'redo', '|', 'findAndReplace', 'showBlocks', 'sourceEditing', '|',
            'heading', '|', 'fontFamily', 'fontSize', 'fontColor', 'fontBackgroundColor', 'highlight', '|',
            'bold', 'italic', 'underline', 'strikethrough', 'subscript', 'superscript', 'code', 'removeFormat', '|',
            'alignment', 'bulletedList', 'numberedList', 'todoList', 'outdent', 'indent', '|',
            'link', 'insertImage', 'insertTable', 'mediaEmbed', 'blockQuote', 'codeBlock', 'htmlEmbed',
            'horizontalLine', 'pageBreak', 'specialCharacters'
        ],
        shouldNotGroupWhenFull: false // extra buttons collapse into a "⋮" menu
    },
    image: {
        toolbar: [
            'imageStyle:inline',
            'imageStyle:wrapText',
            'imageStyle:breakText',
            '|',
            'resizeImage',
            '|',
            'toggleImageCaption', 'imageTextAlternative', 'linkImage'
        ],
        insert: {
            type: 'block'
        }
    },
    table: {
        contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells',
            'tableProperties', 'tableCellProperties', 'toggleTableCaption']
    },
    ckfinder: { uploadUrl: '/ckeditor/upload' }
};

export function createEditor(el, overrides = {}) {
    return ClassicEditor.create(el, { ...fullConfig, ...overrides });
}

export function initEditors(selector = 'textarea.editor', overrides = {}) {
    const instances = {};
    document.querySelectorAll(selector).forEach(el => {
        createEditor(el, overrides)
            .then(editor => { instances[el.id] = editor; })
            .catch(err => console.error(`❌ Error initializing ${el.id}:`, err));
    });
    return instances;
}