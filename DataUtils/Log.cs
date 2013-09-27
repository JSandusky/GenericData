using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DataUtils {

    /* For Some reason I'm dumb and rolled time profiling and msg logging both into one */

    /* Writes out a log in HTML format with styles for different msg levels
     * red = error, yellow = warning, pink = debug, vanilla = general info*/
    public class Log {
        static Dictionary<string, Log> insts_;

        class Msg {
            public int level;
            public string msg;
            public DateTime time;
        }

        List<Msg> messages_ = new List<Msg>();

        Log(string name) {
            logName = name;
        }

        string logName { get; set; }

        public static Log getInst(string name) {
            if (insts_ == null) {
                insts_ = new Dictionary<string, Log>();
                Log l = new Log(name);
                insts_[name] = l;
                return l;
            }

            if (insts_.ContainsKey(name))
                return insts_[name];
            Log log = new Log(name);
            insts_[name] = log;
            return log;
        }

        void log(string msg, int level) {
            messages_.Add(new Msg { level = level, msg = msg, time = DateTime.Now });
        }

        public void info(string msg) {
            log(msg, 3);
        }

        public void debug(string msg) {
            log(msg, 4);
        }

        public void warning(string msg) {
            log(msg, 2);
        }

        public void error(string msg) {
            log(msg, 1);
        }

        public void write(string fp, int minLevel) {
            StringBuilder sb = new StringBuilder();
            if (!File.Exists(fp)) {
                File.Create(fp).Close();
                sb.Append("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html\"; charset=\"utf-8\" />");
                sb.Append("<title>Profile Dump</title><style type=\"text/css\">");

                sb.Append("body, html {\n");
                sb.Append("background: #000000;\n");
                sb.Append("width: 100%;\n");
                sb.Append("font-family: Arial;\n");
                sb.Append("font-size: 16px;\n");
                sb.Append("color: #C0C0C0;\n");
                sb.Append("}\n");

                sb.Append("h1 {\n");
                sb.Append("color : #FFFFFF;\n");
                sb.Append("border-bottom : 1px dotted #888888;\n");
                sb.Append("}\n");

                sb.Append("pre {\n");
                sb.Append("font-family : arial;\n");
                sb.Append("margin : 0;\n");
                sb.Append("}\n");

                sb.Append(".box {\n");
                sb.Append("border : 1px dotted #818286;\n");
                sb.Append("padding : 5px;\n");
                sb.Append("margin: 5px;\n");
                sb.Append("width: 100%;\n");
                sb.Append("background-color : #292929;\n");
                sb.Append("}\n");

                sb.Append(".err {\n");
                sb.Append("color: #EE1100;\n");
                sb.Append("font-weight: bold\n");
                sb.Append("}\n");

                sb.Append(".warn {\n");
                sb.Append("color: #FFCC00;\n");
                sb.Append("font-weight: bold\n");
                sb.Append("}\n");

                sb.Append(".info {\n");
                sb.Append("color: #C0C0C0;\n");
                sb.Append("}\n");

                sb.Append(".debug {\n");
                sb.Append("color: #CCA0A0;\n");
                sb.Append("}\n");


                sb.Append("</style>");
                sb.Append("</head>");
                sb.Append("<body>");

                sb.Append("<h1>Log: " + logName + "</h1>\n");
                sb.Append("<div class=\"box\">\n");
                sb.Append("<table>\n");

            }

            foreach (Msg m in messages_) {
                if (m.level > minLevel)
                    continue;
                sb.Append("<tr>\n");
                sb.Append("<td valign=\"top\" width=\"100\">");
                sb.Append(m.time.ToString());
                sb.Append("</td>\n");
                sb.Append("<td valign=\"top\" class=\"");
                switch (m.level) {
                    case 1:
                        sb.Append("err");
                        break;
                    case 2:
                        sb.Append("warn");
                        break;
                    case 3:
                        sb.Append("info");
                        break;
                    default:
                        sb.Append("debug");
                        break;
                }

                sb.Append("\"><pre>\n");
                sb.Append(m.msg);
                sb.Append("\n</pre></td>\n");
                sb.Append("</tr>\n");
            }
            messages_.Clear();

            using (StreamWriter outfile = new StreamWriter(fp, true)) {
                outfile.Write(sb.ToString());
            }
        }
    }
}