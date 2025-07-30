namespace Food_Haven.Web.Services
{
    public static class TemplateSendmail
    {

        public static string TemplateVerifyLinkCode(string fullName, string Link)
        {
            string body = $"<!doctype html>\n" +
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
  "                            <h5 style=\"font-style: italic;\">Hello, <strong>" + fullName + "</strong></h5>\n" +
  "                            <p>We're excited to have you get started. First, you need to verify your account. Just press the button below.</p>\n" +
  "                        </td>\n" +
  "                    </tr>\n" +
  "                    <tr>\n" +
  "                        <td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\">\n" +
  "                            <a href=\"" + Link + "\" style=\"font-size: 25px; color: #ffffff; background-color: #FFA73B; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #FFA73B; display: inline-block;\">\n" +
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

        public static string TemplateResetPassword(string Link)
        {
            return "<!DOCTYPE html>\n" +
"<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"en\">\n" +
"\n" +
"<head>\n" +
"    <title></title>\n" +
"    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n" +
"    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">\n" +
"    <!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->\n" +
"    <style>\n" +
"        * {\n" +
"            box-sizing: border-box\n" +
"        }\n" +
"\n" +
"        body {\n" +
"            margin: 0;\n" +
"            padding: 0\n" +
"        }\n" +
"\n" +
"        a[x-apple-data-detectors] {\n" +
"            color: inherit !important;\n" +
"            text-decoration: inherit !important\n" +
"        }\n" +
"\n" +
"        #MessageViewBody a {\n" +
"            color: inherit;\n" +
"            text-decoration: none\n" +
"        }\n" +
"\n" +
"        p {\n" +
"            line-height: inherit\n" +
"        }\n" +
"\n" +
"        .desktop_hide,\n" +
"        .desktop_hide table {\n" +
"            mso-hide: all;\n" +
"            display: none;\n" +
"            max-height: 0;\n" +
"            overflow: hidden\n" +
"        }\n" +
"\n" +
"        .image_block img+div {\n" +
"            display: none\n" +
"        }\n" +
"\n" +
"        @media (max-width:660px) {\n" +
"            .social_block.desktop_hide .social-table {\n" +
"                display: inline-block !important\n" +
"            }\n" +
"\n" +
"            .image_block img.fullWidth {\n" +
"                max-width: 100% !important\n" +
"            }\n" +
"\n" +
"            .mobile_hide {\n" +
"                display: none\n" +
"            }\n" +
"\n" +
"            .row-content {\n" +
"                width: 100% !important\n" +
"            }\n" +
"\n" +
"            .stack .column {\n" +
"                width: 100%;\n" +
"                display: block\n" +
"            }\n" +
"\n" +
"            .mobile_hide {\n" +
"                min-height: 0;\n" +
"                max-height: 0;\n" +
"                max-width: 0;\n" +
"                overflow: hidden;\n" +
"                font-size: 0\n" +
"            }\n" +
"\n" +
"            .desktop_hide,\n" +
"            .desktop_hide table {\n" +
"                display: table !important;\n" +
"                max-height: none !important\n" +
"            }\n" +
"        }\n" +
"    </style>\n" +
"</head>\n" +
"\n" +
"<body style=\"background-color:#f8f8f9;margin:0;padding:0;-webkit-text-size-adjust:none;text-size-adjust:none\">\n" +
"    <table class=\"nl-container\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"        style=\"mso-table-lspace:0;mso-table-rspace:0;background-color:#f8f8f9\">\n" +
"        <tbody>\n" +
"            <tr>\n" +
"                <td>\n" +
"                    <table class=\"row row-1\" align=\"center\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                        role=\"presentation\" style=\"mso-table-lspace:0;mso-table-rspace:0;background-color:#1aa19c\">\n" +
"                        <tbody>\n" +
"                            <tr>\n" +
"                                <td>\n" +
"                                    <table class=\"row-content stack\" align=\"center\" border=\"0\" cellpadding=\"0\"\n" +
"                                        cellspacing=\"0\" role=\"presentation\"\n" +
"                                        style=\"mso-table-lspace:0;mso-table-rspace:0;color:#000;background-color:#1aa19c;width:640px;margin:0 auto\"\n" +
"                                        width=\"640\">\n" +
"                                        <tbody>\n" +
"                                            <tr>\n" +
"                                                <td class=\"column column-1\" width=\"100%\"\n" +
"                                                    style=\"mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0\">\n" +
"                                                    <table class=\"divider_block block-1\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                                                                        role=\"presentation\" width=\"100%\"\n" +
"                                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                                        <tr>\n" +
"                                                                            <td class=\"divider_inner\"\n" +
"                                                                                style=\"font-size:1px;line-height:1px;border-top:4px solid #1aa19c\">\n" +
"                                                                                <span>&#8202;</span></td>\n" +
"                                                                        </tr>\n" +
"                                                                    </table>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                </td>\n" +
"                                            </tr>\n" +
"                                        </tbody>\n" +
"                                    </table>\n" +
"                                </td>\n" +
"                            </tr>\n" +
"                        </tbody>\n" +
"                    </table>\n" +
"                    <table class=\"row row-2\" align=\"center\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                        role=\"presentation\" style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                        <tbody>\n" +
"                            <tr>\n" +
"                                <td>\n" +
"                                    <table class=\"row-content stack\" align=\"center\" border=\"0\" cellpadding=\"0\"\n" +
"                                        cellspacing=\"0\" role=\"presentation\"\n" +
"                                        style=\"mso-table-lspace:0;mso-table-rspace:0;color:#000;width:640px;margin:0 auto\"\n" +
"                                        width=\"640\">\n" +
"                                        <tbody>\n" +
"                                            <tr>\n" +
"                                                <td class=\"column column-1\" width=\"100%\"\n" +
"                                                    style=\"mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;padding-bottom:5px;padding-top:5px;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0\">\n" +
"                                                    <table class=\"image_block block-1\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"width:100%;padding-right:0;padding-left:0\">\n" +
"                                                                " +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                </td>\n" +
"                                            </tr>\n" +
"                                        </tbody>\n" +
"                                    </table>\n" +
"                                </td>\n" +
"                            </tr>\n" +
"                        </tbody>\n" +
"                    </table>\n" +
"                    <table class=\"row row-3\" align=\"center\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                        role=\"presentation\" style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                        <tbody>\n" +
"                            <tr>\n" +
"                                <td>\n" +
"                                    <table class=\"row-content stack\" align=\"center\" border=\"0\" cellpadding=\"0\"\n" +
"                                        cellspacing=\"0\" role=\"presentation\"\n" +
"                                        style=\"mso-table-lspace:0;mso-table-rspace:0;background-color:#fff;color:#000;width:640px;margin:0 auto\"\n" +
"                                        width=\"640\">\n" +
"                                        <tbody>\n" +
"                                            <tr>\n" +
"                                                <td class=\"column column-1\" width=\"100%\"\n" +
"                                                    style=\"mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0\">\n" +
"                                                    <table class=\"image_block block-1\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\" style=\"width:100%\">\n" +
"                                                                <div class=\"alignment\" align=\"center\"\n" +
"                                                                    style=\"line-height:10px\"><a href=\"www.example.com\"\n" +
"                                                                        target=\"_blank\" style=\"outline:none\"\n" +
"                                                                        tabindex=\"-1\"><img\n" +
"                                                                            src=\"https://d1oco4z2z1fhwp.cloudfront.net/templates/default/4036/___passwordreset.gif\"\n" +
"                                                                            style=\"display:block;height:auto;border:0;max-width:640px;width:100%\"\n" +
"                                                                            width=\"640\" alt=\"Image of lock &amp; key.\"\n" +
"                                                                            title=\"Image of lock &amp; key.\"></a></div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"divider_block block-2\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\" style=\"padding-top:30px\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                                                                        role=\"presentation\" width=\"100%\"\n" +
"                                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                                        <tr>\n" +
"                                                                            <td class=\"divider_inner\"\n" +
"                                                                                style=\"font-size:1px;line-height:1px;border-top:0 solid #bbb\">\n" +
"                                                                                <span>&#8202;</span></td>\n" +
"                                                                        </tr>\n" +
"                                                                    </table>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"paragraph_block block-3\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0;word-break:break-word\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:10px\">\n" +
"                                                                <div\n" +
"                                                                    style=\"color:#555;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;font-size:30px;line-height:120%;text-align:center;mso-line-height-alt:36px\">\n" +
"                                                                    <p style=\"margin:0;word-break:break-word\">\n" +
"                                                                        <span style=\"color:#2b303a;\"><strong>Forgot Your\n" +
"                                                                                Password?</strong></span>\n" +
"                                                                    </p>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"paragraph_block block-4\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0;word-break:break-word\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:10px\">\n" +
"                                                                <div\n" +
"                                                                    style=\"color:#555;font-family:Montserrat,Trebuchet MS,Lucida Grande,Lucida Sans Unicode,Lucida Sans,Tahoma,sans-serif;font-size:15px;line-height:150%;text-align:center;mso-line-height-alt:22.5px\">\n" +
"                                                                    <p style=\"margin:0;word-break:break-word\"><span\n" +
"                                                                            style=\"color:#808389;\"><strong>Note:</strong> This link is only valid for <strong>10 minutes</strong>. Please use it before it expires.</span></p>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"button_block block-5\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-left:10px;padding-right:10px;padding-top:15px;text-align:center\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <!--[if mso]><v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"www.example.com\" style=\"height:62px;width:209px;v-text-anchor:middle;\" arcsize=\"57%\" stroke=\"false\" fillcolor=\"#f7a50c\"><w:anchorlock/><v:textbox inset=\"0px,0px,0px,0px\"><center style=\"color:#ffffff; font-family:Arial, sans-serif; font-size:16px\"><![endif]-->\n" +
"                                                                    <a href=\"" + Link + "\" target=\"_blank\"\n" +
"                                                                        style=\"text-decoration:none;display:inline-block;color:#ffffff;background-color:#f7a50c;border-radius:35px;width:auto;border-top:0px solid transparent;font-weight:undefined;border-right:0px solid transparent;border-bottom:0px solid transparent;border-left:0px solid transparent;padding-top:15px;padding-bottom:15px;font-family:'Helvetica Neue', Helvetica, Arial, sans-serif;font-size:16px;text-align:center;mso-border-alt:none;word-break:keep-all;\"><span\n" +
"                                                                            style=\"padding-left:30px;padding-right:30px;font-size:16px;display:inline-block;letter-spacing:normal;\"><span\n" +
"                                                                                style=\"margin: 0; word-break: break-word; line-height: 32px;\"><strong>RESET\n" +
"                                                                                    PASSWORD</strong></span></span></a>\n" +
"                                                                    <!--[if mso]></center></v:textbox></v:roundrect><![endif]-->\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"divider_block block-6\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:12px;padding-top:60px\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                                                                        role=\"presentation\" width=\"100%\"\n" +
"                                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                                        <tr>\n" +
"                                                                            <td class=\"divider_inner\"\n" +
"                                                                                style=\"font-size:1px;line-height:1px;border-top:0 solid #bbb\">\n" +
"                                                                                <span>&#8202;</span></td>\n" +
"                                                                        </tr>\n" +
"                                                                    </table>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                </td>\n" +
"                                            </tr>\n" +
"                                        </tbody>\n" +
"                                    </table>\n" +
"                                </td>\n" +
"                            </tr>\n" +
"                        </tbody>\n" +
"                    </table>\n" +
"                    <table class=\"row row-4\" align=\"center\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                        role=\"presentation\" style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                        <tbody>\n" +
"                            <tr>\n" +
"                                <td>\n" +
"                                    <table class=\"row-content stack\" align=\"center\" border=\"0\" cellpadding=\"0\"\n" +
"                                        cellspacing=\"0\" role=\"presentation\"\n" +
"                                        style=\"mso-table-lspace:0;mso-table-rspace:0;color:#000;background-color:#410125;width:640px;margin:0 auto\"\n" +
"                                        width=\"640\">\n" +
"                                        <tbody>\n" +
"                                            <tr>\n" +
"                                                <td class=\"column column-1\" width=\"100%\"\n" +
"                                                    style=\"mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0\">\n" +
"                                                    <table class=\"image_block block-1\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"width:100%;padding-right:0;padding-left:0\">\n" +
"                                                                " +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"social_block block-2\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:28px;text-align:center\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <table class=\"social-table\" width=\"208px\" border=\"0\"\n" +
"                                                                        cellpadding=\"0\" cellspacing=\"0\"\n" +
"                                                                        role=\"presentation\"\n" +
"                                                                        style=\"mso-table-lspace:0;mso-table-rspace:0;display:inline-block\">\n" +
"                                                                        <tr>\n" +
"                                                                            <td style=\"padding:0 10px 0 10px\"><a\n" +
"                                                                                    href=\"https://www.facebook.com\"\n" +
"                                                                                    target=\"_blank\"><img\n" +
"                                                                                        src=\"https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/facebook@2x.png\"\n" +
"                                                                                        width=\"32\" height=\"32\"\n" +
"                                                                                        alt=\"Facebook\" title=\"Facebook\"\n" +
"                                                                                        style=\"display:block;height:auto;border:0\"></a>\n" +
"                                                                            </td>\n" +
"                                                                            <td style=\"padding:0 10px 0 10px\">\n" +
"                                                                                <a href=\"https://www.twitter.com\"\n" +
"                                                                                    target=\"_blank\"><img\n" +
"                                                                                        src=\"https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/twitter@2x.png\"\n" +
"                                                                                        width=\"32\" height=\"32\"\n" +
"                                                                                        alt=\"Twitter\" title=\"Twitter\"\n" +
"                                                                                        style=\"display:block;height:auto;border:0\"></a>\n" +
"                                                                            </td>\n" +
"                                                                            <td style=\"padding:0 10px 0 10px\"><a\n" +
"                                                                                    href=\"https://www.instagram.com\"\n" +
"                                                                                    target=\"_blank\"><img\n" +
"                                                                                        src=\"https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/instagram@2x.png\"\n" +
"                                                                                        width=\"32\" height=\"32\"\n" +
"                                                                                        alt=\"Instagram\"\n" +
"                                                                                        title=\"Instagram\"\n" +
"                                                                                        style=\"display:block;height:auto;border:0\"></a>\n" +
"                                                                            </td>\n" +
"                                                                            <td style=\"padding:0 10px 0 10px\"><a\n" +
"                                                                                    href=\"https://www.linkedin.com\"\n" +
"                                                                                    target=\"_blank\"><img\n" +
"                                                                                        src=\"https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/linkedin@2x.png\"\n" +
"                                                                                        width=\"32\" height=\"32\"\n" +
"                                                                                        alt=\"LinkedIn\" title=\"LinkedIn\"\n" +
"                                                                                        style=\"display:block;height:auto;border:0\"></a>\n" +
"                                                                            </td>\n" +
"                                                                        </tr>\n" +
"                                                                    </table>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"divider_block block-3\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:25px\">\n" +
"                                                                <div class=\"alignment\" align=\"center\">\n" +
"                                                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\n" +
"                                                                        role=\"presentation\" width=\"100%\"\n" +
"                                                                        style=\"mso-table-lspace:0;mso-table-rspace:0\">\n" +
"                                                                        <tr>\n" +
"                                                                            <td class=\"divider_inner\"\n" +
"                                                                                style=\"font-size:1px;line-height:1px;border-top:1px solid #555961\">\n" +
"                                                                                <span>&#8202;</span></td>\n" +
"                                                                        </tr>\n" +
"                                                                    </table>\n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                    <table class=\"paragraph_block block-4\" width=\"100%\" border=\"0\"\n" +
"                                                        cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"\n" +
"                                                        style=\"mso-table-lspace:0;mso-table-rspace:0;word-break:break-word\">\n" +
"                                                        <tr>\n" +
"                                                            <td class=\"pad\"\n" +
"                                                                style=\"padding-bottom:30px;padding-left:40px;padding-right:40px;padding-top:20px\">\n" +
"                                                                <div\n" +
"                                                                    style=\"color:#555;font-family:Montserrat,Trebuchet MS,Lucida Grande,Lucida Sans Unicode,Lucida Sans,Tahoma,sans-serif;font-size:12px;line-height:120%;text-align:center;mso-line-height-alt:14.399999999999999px\">\n" +
"                                                                    <p style=\"margin:0;word-break:break-word\"><span\n" +
"                                                                            style=\"color:#95979c;\">Copyright ©\n" +
"                                                                            2021</span></p>\n" +
"                                                                    \n" +
"                                                                   \n" +
"                                                                </div>\n" +
"                                                            </td>\n" +
"                                                        </tr>\n" +
"                                                    </table>\n" +
"                                                </td>\n" +
"                                            </tr>\n" +
"                                        </tbody>\n" +
"                                    </table>\n" +
"                                </td>\n" +
"                            </tr>\n" +
"                        </tbody>\n" +
"                    </table>\n" +
"                </td>\n" +
"            </tr>\n" +
"        </tbody>\n" +
"    </table><!-- End -->\n" +
"</body>\n" +
"\n" +
"</html>";
        }
    }
}
