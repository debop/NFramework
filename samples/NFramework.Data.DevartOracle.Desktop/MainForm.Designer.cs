namespace NSoft.NFramework.Data.DevartOracle.Desktop
{
	partial class MainForm
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.cmdLoadCompany = new System.Windows.Forms.Button();
			this.lblMessage = new System.Windows.Forms.Label();
			this.oracleMonitor1 = new Devart.Data.Oracle.OracleMonitor();
			this.SuspendLayout();
			// 
			// cmdLoadCompany
			// 
			this.cmdLoadCompany.Location = new System.Drawing.Point(97, 135);
			this.cmdLoadCompany.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cmdLoadCompany.Name = "cmdLoadCompany";
			this.cmdLoadCompany.Size = new System.Drawing.Size(120, 44);
			this.cmdLoadCompany.TabIndex = 0;
			this.cmdLoadCompany.Text = "Load Company";
			this.cmdLoadCompany.UseVisualStyleBackColor = true;
			this.cmdLoadCompany.Click += new System.EventHandler(this.cmdLoadCompany_Click);
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Location = new System.Drawing.Point(12, 46);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(108, 15);
			this.lblMessage.TabIndex = 1;
			this.lblMessage.Text = "Company Count =";
			// 
			// oracleMonitor1
			// 
			this.oracleMonitor1.IsActive = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 196);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.cmdLoadCompany);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "DevartOracle License Demo";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdLoadCompany;
		private System.Windows.Forms.Label lblMessage;
		private Devart.Data.Oracle.OracleMonitor oracleMonitor1;
	}
}

