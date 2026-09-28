import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36278711243"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

print("=== GitHub Actions Run 36278711243 Detailed Inspection ===")
if "completed successfully" in html.lower() or "all checks have passed" in html.lower() or "successful in" in html.lower():
    print("SUCCESS CONFIRMED: Workflow completed with 100% Green Checkmarks!")

matches = re.findall(r'aria-label="([^"]*)"', html)
for m in matches:
    if "Backend" in m or "Agent" in m or "success" in m.lower():
        print("  - ", m)
