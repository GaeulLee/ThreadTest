using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ThreadTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 윈도우 폼에서의 다중 스레드
        // 크로스 스레드 발생 -> delegate와 invoke에 대해 알아야 함
        private void Save_btn_Click(object sender, EventArgs e)
        {
            // 텍스트 입력 유효성 확인
            if (textBox2.Text.Equals("") || textBox2 == null)
            {
                MessageBox.Show("저장할 파일명을 적어주세요.");
                return;
            }
            if (textBox1.Text.Equals("") || textBox1 == null)
            {
                MessageBox.Show("저장할 내용을 적어주세요.");
                return;
            }

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            // 스레드 호출(파일 저장 및 진행바)
            Thread saveFileThr = new Thread(saveFile);
            Thread pBarThr = new Thread(pBar);

            saveFileThr.Start();
            pBarThr.Start();

            // 스레드 수행 완료 후 메인 스레드와 합류
            pBarThr.Join();
            saveFileThr.Join();
            MessageBox.Show("파일 저장 완료.");

            // textbox, progress bar reset
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            label3.Text = string.Empty;
            progressBar1.Value = 0;
        }

        private void saveFile() // 여기에는 왜 델리게이트와 invoke가 필요 없는가? 컨트롤을 생성하지 않았기 때문
        {
            StreamWriter writer;
            writer = File.CreateText(@"C:\Users\LeeGaeul\Desktop\" + textBox2.Text + ".txt");
            //Text File이 저장될 위치(파일명), 파일 이름만 지정하면 컴파일된 폴더내 해당 파일 이름으로 저장됨
            writer.WriteLine(textBox1.Text); // 저장될 string
            writer.Close();
        }

        private void pBar()
        {
            for (int i=0; i<progressBar1.Maximum; i++)
            {
                SetProgressValue(i);
            /*
                progressBar1.Value = var;
                label3.Text = ((int)var + 1).ToString() + "%";
            */
                Thread.Sleep(5); // 진행바 속도 조절
            }
        }

        delegate void pBarDel(int var); // 대리자를 쓰는 이유가 무엇인가?
        private void SetProgressValue(int var) // 이 과정이 정말 이해가 안간다..
        {
            // InvokeRequired => 크로스 스레드 발생 원인의 값을 구분 지을 수 있음
            // true이면, 현재 스레드가 자신을 생성한 스레드가 아닌 것
            // false이면, 현재 스레드가 자신을 생성한 스레드인 것
            if (progressBar1.InvokeRequired)
            {
                pBarDel del = new pBarDel(SetProgressValue); // 대리자 객체 생성
                progressBar1.BeginInvoke(del, var);
            }
            else
            {
                progressBar1.Value = var;
                label3.Text = ((int)var + 1).ToString() + "%";
            }       
        }

        
    }
}
