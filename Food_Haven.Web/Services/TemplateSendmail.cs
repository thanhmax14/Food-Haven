namespace Food_Haven.Web.Services
{
    public static class TemplateSendmail
    {

        public static string TemplateVerifyLinkCode(string fullName, string Link)
        {
          string  body = $"<!doctype html>\n" +
"<html>\n" +
"\n" +
"<head>\n" +
"    <meta charset='utf-8'>\n" +
"    <meta name='viewport' content='width=device-width, initial-scale=1'>\n" +
"    <title>Welcome Email</title>\n" +
"    <link href='https://stackpath.bootstrapcdn.com/bootstrap/5.0.0-alpha1/css/bootstrap.min.css' rel='stylesheet'>\n" +
"    <style>\n" +
"        @media screen {\n" +
"            @font-face {\n" +
"                font-family: 'Lato';\n" +
"                font-style: normal;\n" +
"                font-weight: 400;\n" +
"                src: url(https://fonts.gstatic.com/s/lato/v11/qIIYRU-oROkIk8vfvxw6QvesZW2xOQ-xsNqO47m55DA.woff) format('woff');\n" +
"            }\n" +
"\n" +
"            @font-face {\n" +
"                font-family: 'Lato';\n" +
"                font-style: normal;\n" +
"                font-weight: 700;\n" +
"                src: url(https://fonts.gstatic.com/s/lato/v11/qdgUG4U09HnJwhYI-uK18wLUuEpTyoUstqEm5AMlJo4.woff) format('woff');\n" +
"            }\n" +
"\n" +
"            @font-face {\n" +
"                font-family: 'Lato';\n" +
"                font-style: italic;\n" +
"                font-weight: 400;\n" +
"                src: url(https://fonts.gstatic.com/s/lato/v11/RYyZNoeFgb0l7W3Vu1aSWOvvDin1pK8aKteLpeZ5c0A.woff) format('woff');\n" +
"            }\n" +
"\n" +
"            @font-face {\n" +
"                font-family: 'Lato';\n" +
"                font-style: italic;\n" +
"                font-weight: 700;\n" +
"                src: url(https://fonts.gstatic.com/s/lato/v11/HkF_qI1x_noxlxhrhMQYELO3LdcAZYWl9Si6vvxL-qU.woff) format('woff');\n" +
"            }\n" +
"        }\n" +
"\n" +
"        body,\n" +
"        table,\n" +
"        td,\n" +
"        a {\n" +
"            -webkit-text-size-adjust: 100%;\n" +
"            -ms-text-size-adjust: 100%;\n" +
"        }\n" +
"\n" +
"        table,\n" +
"        td {\n" +
"            mso-table-lspace: 0pt;\n" +
"            mso-table-rspace: 0pt;\n" +
"        }\n" +
"\n" +
"        img {\n" +
"            -ms-interpolation-mode: bicubic;\n" +
"            border: 0;\n" +
"            height: auto;\n" +
"            line-height: 100%;\n" +
"            outline: none;\n" +
"            text-decoration: none;\n" +
"        }\n" +
"\n" +
"        table {\n" +
"            border-collapse: collapse !important;\n" +
"        }\n" +
"\n" +
"        body {\n" +
"            height: 100% !important;\n" +
"            margin: 0 !important;\n" +
"            padding: 0 !important;\n" +
"            width: 100% !important;\n" +
"            background-color: #f4f4f4;\n" +
"        }\n" +
"\n" +
"        a[x-apple-data-detectors] {\n" +
"            color: inherit !important;\n" +
"            text-decoration: none !important;\n" +
"        }\n" +
"\n" +
"        @media screen and (max-width:600px) {\n" +
"            h1 {\n" +
"                font-size: 32px !important;\n" +
"                line-height: 32px !important;\n" +
"            }\n" +
"        }\n" +
"\n" +
"        div[style*=\"margin: 16px 0;\"] {\n" +
"            margin: 0 !important;\n" +
"        }\n" +
"    </style>\n" +
"</head>\n" +
"\n" +
"<body oncontextmenu='return false' class='snippet-body'>\n" +
"    <div style=\"display: none; font-size: 1px; color: #fefefe; line-height: 1px; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;\">\n" +
"        We're thrilled to have you here! Get ready to dive into your new account.\n" +
"    </div>\n" +
"    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\n" +
"        <tr>\n" +
"            <td bgcolor=\"#FFA73B\" align=\"center\">\n" +
"                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n" +
"                    <tr>\n" +
"                        <td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\"></td>\n" +
"                    </tr>\n" +
"                </table>\n" +
"            </td>\n" +
"        </tr>\n" +
"        <tr>\n" +
"            <td bgcolor=\"#FFA73B\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\n" +
"                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#ffffff\" align=\"center\" valign=\"top\"\n" +
"                            style=\"padding: 40px 20px 20px 20px; border-radius: 4px 4px 0px 0px; font-size: 48px; font-weight: 400; letter-spacing: 4px;\">\n" +
"                            <h1>Welcome!</h1>\n" +
"                            <img src=\"https://img.icons8.com/clouds/100/000000/handshake.png\" width=\"125\" height=\"120\" style=\"display: block; border: 0px;\" />\n" +
"                        </td>\n" +
"                    </tr>\n" +
"                </table>\n" +
"            </td>\n" +
"        </tr>\n" +
"        <tr>\n" +
"            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\n" +
"                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; font-size: 18px; font-weight: 400; line-height: 25px;\">\n" +
"                            <h5 style=\"font-style: italic;\">Hello, <strong>"+fullName+"</strong></h5>\n" +
"                            <p>We're excited to have you get started. First, you need to verify your account. Just press the button below.</p>\n" +
"                        </td>\n" +
"                    </tr>\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\">\n" +
"                            <a href=\""+Link+"\" style=\"font-size: 25px; color: #ffffff; background-color: #FFA73B; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #FFA73B; display: inline-block;\">\n" +
"                              Verify Confirm link\n" +
"                            </a>\n" +
"                        </td>\n" +
"                    </tr>\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 20px 30px; font-size: 18px; line-height: 25px;\">\n" +
"                            <p>If you have any questions, just reply to this email—we're always happy to help out.</p>\n" +
"                            <p><strong>Note:</strong> This verification link is only valid for 10 minutes. Please complete the verification as\n" +
"                                soon as possible.</p>\n" +
"                        </td>\n" +
"                        \n" +
"                    </tr>\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 40px 30px; font-size: 18px; line-height: 25px;\">\n" +
"                            <p>Cheers,<br>FH-Food Haven</p>\n" +
"                        </td>\n" +
"                    </tr>\n" +
"                </table>\n" +
"            </td>\n" +
"        </tr>\n" +
"        <tr>\n" +
"            <td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 30px 10px 0px 10px;\">\n" +
"                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n" +
"                    <tr>\n" +
"                        <td bgcolor=\"#FFECD1\" align=\"center\" style=\"padding: 30px; font-size: 18px;\">\n" +
"                            <h2 style=\"font-size: 20px;\">Need more help?</h2>\n" +
"                            <p><a href=\"https://www.facebook.com/thanhmax1414/\" target=\"_blank\" style=\"color: #FFA73B;\">Chat With Admin to Support</a></p>\n" +
"                        </td>\n" +
"                    </tr>\n" +
"                </table>\n" +
"            </td>\n" +
"        </tr>\n" +
"    </table>\n" +
"</body>\n" +
"\n" +
"</html>";
            return body;
        }
    }
}
