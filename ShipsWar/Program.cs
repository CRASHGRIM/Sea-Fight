using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipsWar
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(new ModelGame(AIMap)) { ClientSize = new Size(1000, 1000) });
        }


        public const string AIMap = @"
****     *
          
    *     
    *     
        **
    *     
 *  *  ***
          
         *
***   *   ";
    }
}
