import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36278711243"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

# Search for job titles and check status icons
jobs = re.findall(r'class="[^"]*JobStep[^"]*"[^>]*>(.*?)</div>', html, re.DOTALL)
print("HTML length:", len(html))

# Look for 'failure', 'error', 'failed', 'successful', 'passed' in check run details
for line in html.splitlines():
    if any(k in line.lower() for k in ["job", "backend", "agent", "build", "test", "conclusion"]):
        if len(line.strip()) < 150:
            print(line.strip())
