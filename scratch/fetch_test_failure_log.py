import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36279732053"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

job_urls = re.findall(r'href="(/IT24104023/ClearToWork/actions/runs/36279732053/job/\d+)"', html)
print("Job URLs:", list(dict.fromkeys(job_urls)))

for ju in list(dict.fromkeys(job_urls)):
    full_url = "https://github.com" + ju
    jhtml = urllib.request.urlopen(urllib.request.Request(full_url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')
    print("\n=== JOB:", ju, "===")
    matches = re.findall(r'aria-label="([^"]*annotation[^"]*)"', jhtml)
    print("Annotations:", matches)
    for line in jhtml.splitlines():
        if any(k in line.lower() for k in ["failed", "error", "assert", "expected", "exception"]):
            clean_l = re.sub(r'<[^>]+>', ' ', line).strip()
            if len(clean_l) > 10 and len(clean_l) < 300:
                print("  LINE:", clean_l)
